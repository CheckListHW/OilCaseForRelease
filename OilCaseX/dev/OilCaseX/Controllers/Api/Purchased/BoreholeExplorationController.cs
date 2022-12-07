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
        public IActionResult Post([FromBody] ApiModels.PurchasedBoreholeExplorationPost value)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            if (value.TrajectoryPoints is { Count: 2 })
                return Conflict("Необходимо указать 2 точки траектории");

            var kust = _context.PurchasedObject
                .Include(po => po.ObjectOfArrangement)
                .FirstOrDefault(po => po.TeamId == team.Id
                                      & po.Id == value.PurchasedObjectOfArrangementId
                                      & po.ObjectOfArrangement.Key == "soKust");

            if (kust == null) return Conflict("Необходимо купить кустовую площадку");

            var purchasedBorehole = new DbModels.PurchasedBoreholeExploration()
            {
                PurchasedObjectOfArrangement = kust,
                Team = team,
                GameStep = team.GameStep,
                TrajectoryPoints = value.TrajectoryPoints.Select(tp => new DbModels.TrajectoryPoint()
                {
                    CellX = tp.X,
                    CellY = tp.Y,
                    CellZ = tp.Z,
                })
            };

            try
            {
                _context.PurchasedBoreholeExplorations.Add(purchasedBorehole);
                _context.SaveChanges();
                return Created("", purchasedBorehole.Id);
            }
            catch (Exception r)
            {
                Console.WriteLine(r);
                return Conflict(r);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ApiModels.PurchasedBoreholeExplorationGet>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var boreholes = _context.PurchasedBoreholeExplorations
                .Include(pb => pb.TrajectoryPoints)
                .Select(pb => new ApiModels.PurchasedBoreholeExplorationGet()
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

            var pb = _context.PurchasedBoreholeExplorations.Find(valuePatch.Id);
            if (pb == null) return NotFound();

            if (pb.GameStep != team.GameStep) return Conflict("Скважина не доступна к изменению. Ход уже завершен");

            var allLogNames = _context.LogNames.Where(ln => ln.LithologicalModelId == team.LithologicalModelId);
            pb.PurchasedLogNames = new List<DbModels.PurchasedLogName>();

            foreach (var name in valuePatch.LogNames)
            {
                DbModels.LogName? logName;
                if ((logName = allLogNames.FirstOrDefault(ln => ln.Name == name)) != null )
                    pb.PurchasedLogNames.Add(new(){
                        LogName = logName,
                        PurchasedBoreholeExploration = pb,
                    });
            }

            try
            {
                _context.Update(pb);
                _context.SaveChanges();
                return Accepted();
            }
            catch (Exception e)
            {
                return Conflict(e);
            }
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

            var purchasedBorehole = _context.PurchasedBoreholeExplorations.Find(id);
            if (purchasedBorehole?.TeamId != team.Id) return NotFound();

            if (purchasedBorehole.GameStep != team.GameStep) return Conflict("Уже нельзя удалить скважину");

            _context.PurchasedBoreholeExplorations.Remove(purchasedBorehole);
            _context.SaveChanges();
            return Accepted();
        }
    }
}