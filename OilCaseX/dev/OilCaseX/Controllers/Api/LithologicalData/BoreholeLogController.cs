using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiModels = OilCaseApi.Controllers.ApiModels;
using DbModels = OilCaseApi.Models;

namespace OilCaseApi.Controllers.Api.LithologicalData
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class BoreholeLogController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public BoreholeLogController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string userName) => _context.Users
            .Include(u => u.Team)
            .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// Возвращает скважины и купленные для нее гисы 
        /// </summary>
        /// <response code="200">Скважины и гисы</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApiModels.Borehole>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModels.Team? team;
            if ((team = GetUser(User.Claims.FirstOrDefault().Value)?.Team) == null) return Unauthorized();

            var purchasedBoreholes = _context.PurchasedBoreholes
                .Include(u => u.TrajectoryPoints)
                .Include(u => u.PurchasedObjectOfArrangement)
                .Include(u => u.PurchasedLogNames)
                .ThenInclude(pln => pln.LogName)
                .Where(pln => pln.TeamId == team.Id & pln.GameStep < team.GameStep);

            var boreholesApi = new List<ApiModels.Borehole>();
            foreach (var borehole in purchasedBoreholes)
            {
                ApiModels.Borehole boreholeApiApi = new()
                {
                    Id = borehole.Id,
                    Name = borehole.Name,
                    X = borehole.PurchasedObjectOfArrangement.CellX,
                    Y = borehole.PurchasedObjectOfArrangement.CellY,
                    ZMax = borehole.TrajectoryPoints.Max(tp => tp.CellZ),
                    ZMin = borehole.TrajectoryPoints.Min(tp => tp.CellZ)
                };

                DbModels.Cell? cell = _context.Cells.FirstOrDefault(c =>
                    c.X == boreholeApiApi.X & c.Y == boreholeApiApi.Y &
                    c.LithologicalModelId == team.LithologicalModelId);

                if (cell == null) continue;

                var logNames = borehole.PurchasedLogNames.Select(pln => pln.LogNameId);
                var boreholeLogs = _context.BoreholeLogs
                    .Where(bl => bl.CellId == cell.Id)
                    .Where(bl => logNames.Contains(bl.LogNameId))
                    .Select(bl => new
                    {
                        bl.LogNameId,
                        bl.Value,
                        bl.Z,
                    })
                    .ToList();

                foreach (var purchasedLogName in borehole.PurchasedLogNames)
                {
                    var bhl = boreholeLogs
                        .Where(bl => bl.LogNameId == purchasedLogName.LogNameId)
                        .OrderByDescending(bl => bl.Z);
                    boreholeApiApi.BoreholeLog.Add(new ApiModels.BoreholeLog()
                    {
                        Name = purchasedLogName.LogName.Name,
                        Description = purchasedLogName.LogName.Description,
                        Positions = bhl.Select(bl => bl.Z).ToList(),
                        Values = bhl.Select(bl => bl.Value).ToList(),
                    });
                }

                boreholesApi.Add(boreholeApiApi);
            }

            return Ok(boreholesApi);
        }
    }
}