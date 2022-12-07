using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiModels = OilCaseApi.Controllers.ApiModels;
using DbModels = OilCaseApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/Purchased/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class BoreholeExplorationController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public BoreholeExplorationController(DbModels.ApplicationContext context)
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
        public IActionResult Post([FromBody] ApiModels.PurchasedBorehole value)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            if (value.TrajectoryPoints is not { Count: 2 })
                return Conflict("Необходимо указать 2 точки траектории");

            var firsPoint = value.TrajectoryPoints.First();
            var kusts = _context.PurchasedObject
                .Include(po => po.ObjectOfArrangement)
                .Where(po => po.TeamId == team.Id
                             & po.CellX == firsPoint.X
                             & po.CellY == firsPoint.Y
                             & po.ObjectOfArrangement.Key == "soKust");

            if (!kusts.Any()) return Conflict("Необходимо купить кустовую площадку");
            var kust = kusts.First();

            var purchasedBorehole = new DbModels.PurchasedBorehole()
            {
                Name = $"e_{firsPoint.X}_{firsPoint.Y}",
                IsProduction = false,
                PurchasedObjectOfArrangement = kust,
                Team = team,
                GameStep = team.GameStep,
                TrajectoryPoints = value.TrajectoryPoints.Select(tp => new DbModels.TrajectoryPoint()
                {
                    CellX = tp.X,
                    CellY = tp.Y,
                    CellZ = tp.Z,
                }).ToList()
            };

            _context.PurchasedBoreholes.Add(purchasedBorehole);
            _context.SaveChanges();
            return Created("", purchasedBorehole.Id);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ApiModels.PurchasedBoreholeGet>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var boreholes = _context.PurchasedBoreholes
                .Include(pb => pb.TrajectoryPoints)
                .Where(pb => pb.TeamId == team.Id & pb.IsProduction == false)
                .Select(pb => new ApiModels.PurchasedBoreholeGet()
                {
                    Id = pb.Id,
                    Name = pb.Name,
                    GameStep = pb.GameStep,
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
        public IActionResult Patch([FromBody] ApiModels.PurchasedBoreholeExplorationPatch valuePatch)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var pb = _context.PurchasedBoreholes.Find(valuePatch.Id);
            if (pb == null) return NotFound();

            if (pb.GameStep != team.GameStep) return Conflict("Скважина не доступна к изменению. Ход уже завершен");

            var allLogNames = _context.LogNames
                .Where(ln => ln.LithologicalModelId == team.LithologicalModelId);

            _context.PurchasedLogName.RemoveRange(_context.PurchasedLogName.Where(pln => pln.PurchasedBoreholeId == pb.Id));
            _context.SaveChanges();

            pb.PurchasedLogNames = new List<DbModels.PurchasedLogName>();

            foreach (var name in valuePatch.LogNames)
            {
                DbModels.LogName? logName;
                if ((logName = allLogNames.FirstOrDefault(ln => ln.Name == name)) != null)
                    pb.PurchasedLogNames.Add(new()
                    {
                        LogName = logName,
                        PurchasedBorehole = pb,
                    });
            }

            _context.Update(pb);
            _context.SaveChanges();
            return Accepted();
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