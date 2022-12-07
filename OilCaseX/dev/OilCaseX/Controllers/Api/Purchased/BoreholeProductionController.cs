using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModels = OilCaseApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/Purchased/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class BoreholeProductionController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public BoreholeProductionController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string userName)
            => _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// При создании возвращает id созданного объекта
        /// </summary>
        /// <param name="value"></param>
        /// <response code="200">Return id object</response>
        /// <returns>id object</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status409Conflict)]
        public IActionResult Post([FromBody] ApiModels.PurchasedBoreholeProductionPost value)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            if (value.TrajectoryPoints is { Count: < 2 })
                return Conflict("Необходимо указать минимум 2 точки траектории");

            var firsPoint = value.TrajectoryPoints.First();
            var kusts = _context.PurchasedObject
                .Include(po => po.ObjectOfArrangement)
                .Where(po => po.TeamId == team.Id
                             & po.CellX == firsPoint.X
                             & po.CellY == firsPoint.Y
                             & po.ObjectOfArrangement.Key == "soKust");

            if (!kusts.Any()) return Conflict("Необходимо купить кустовую площадку");
            var kust = kusts.First();

            var boreholeStatus = _context.BoreholeStatus.Find(1);

            var purchasedBorehole = new DbModels.PurchasedBorehole()
            {
                Name = $"p_{firsPoint.X}_{firsPoint.Y}",
                IsProduction = true,
                PurchasedObjectOfArrangement = kust,
                Team = team,
                GameStep = team.GameStep,
                TrajectoryPoints = value.TrajectoryPoints.Select(tp => new DbModels.TrajectoryPoint()
                {
                    CellX = tp.X,
                    CellY = tp.Y,
                    CellZ = tp.Z,
                }).ToList(),
                BoreholeStatusHistories = new()
                {
                    new()
                    {
                        BoreholeStatus = boreholeStatus,
                        GameStep = team.GameStep
                    }
                }
            };

            _context.PurchasedBoreholes.Add(purchasedBorehole);
            _context.SaveChanges();
            return Created("", purchasedBorehole.Id);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ApiModels.PurchasedBoreholeProductionGet>),
            statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var boreholes = _context.PurchasedBoreholes
                .Include(pb => pb.TrajectoryPoints)
                .Include(pb => pb.BoreholeStatusHistories)
                .Where(pb => pb.IsProduction == true & pb.TeamId == team.Id)
                .Select(pb => new ApiModels.PurchasedBoreholeProductionGet()
                {
                    Id = pb.Id,
                    Name = pb.Name,
                    GameStep = pb.GameStep,
                    BoreholeStatusId = pb.BoreholeStatusHistories.Max(bsh => bsh.GameStep),
                    TrajectoryPoints = pb.TrajectoryPoints.Select(p =>
                        new ApiModels.TrajectoryPoint()
                        {
                            X = p.CellX,
                            Y = p.CellY,
                            Z = p.CellZ,
                        }).ToList()
                });

            return Ok(boreholes);
        }


        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status409Conflict)]
        public IActionResult Patch([FromBody] ApiModels.PurchasedBoreholeProductionPatch valuePatch)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var pb = _context.PurchasedBoreholes.Find(valuePatch.Id);
            if (pb == null) return NotFound();

            if (pb.TeamId != team.Id) return NotFound();

            var boreholeStatus = _context.BoreholeStatus.Find(valuePatch.StatusId);
            if (boreholeStatus == null)
                return Conflict("Неверный статус");

            var oldStatus = _context.BoreholeStatusHistories
                .FirstOrDefault(bsh => bsh.GameStep == team.GameStep & bsh.PurchasedBoreholeId == valuePatch.Id);
            if (oldStatus == null)
            {
                var newStatus = new DbModels.BoreholeStatusHistory()
                {
                    BoreholeStatusId = boreholeStatus.Id,
                    GameStep = team.GameStep,
                    PurchasedBoreholeId = valuePatch.Id,
                };
                _context.BoreholeStatusHistories.Add(newStatus);
            }
            else
            {
                oldStatus.BoreholeStatusId = boreholeStatus.Id;
                _context.BoreholeStatusHistories.Update(oldStatus);
            }

            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Удалять скважины можно только созданные на текущем ходу
        /// </summary>
        /// <param name="id"></param>
        /// <response code="409">Уже нельзя удалить скважину</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status409Conflict)]
        public IActionResult Delete(int id)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var purchasedBorehole = _context.PurchasedBoreholes.Find(id);
            if (purchasedBorehole?.TeamId != team.Id) return NotFound();

            if (purchasedBorehole.GameStep != team.GameStep) return Conflict("Уже нельзя удалить скважину");

            _context.PurchasedBoreholes.Remove(purchasedBorehole);
            _context.SaveChanges();
            return Accepted();
        }
    }
}