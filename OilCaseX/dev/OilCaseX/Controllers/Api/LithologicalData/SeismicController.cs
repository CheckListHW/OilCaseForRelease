using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModel = OilCaseApi.Models;

namespace OilCaseApi.Controllers.Api.LithologicalData
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class SeismicController : ControllerBase
    {
        private readonly DbModel.ApplicationContext _context;

        public SeismicController(DbModel.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModel.User? GetUser(string? userName)
            => _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// Данные по купленным гисам
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApiModels.Seismic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModel.Team? team;
            if ((team = GetUser(User.Claims.FirstOrDefault()?.Value)?.Team) == null) 
                return Unauthorized();
            
            var purchasedSeismic = _context.PurchasedSeismic.Where(ps => ps.TeamId == team.Id);

            var seismics = _context.Seismic
                .Where(c => c.LithologicalModelId == team.LithologicalModelId)
                .ToList();

            var arraySurfaceApi = new List<ApiModels.Seismic>();
            foreach (var item in purchasedSeismic)
            {
                ApiModels.Seismic surfaceApi = new()
                {
                    StartX = item.StartCellX,
                    StartY = item.StartCellY,
                    EndX = item.EndCellX,
                    EndY = item.EndCellY,
                };

                var isVertical = item.StartCellX == item.EndCellX;
                var seismicLine = isVertical
                    ? seismics.Where(s => s.X == item.EndCellX).OrderBy(s => s.Y) 
                    : seismics.Where(s => s.Y == item.EndCellY).OrderBy(s => s.X);
                
                foreach (var seismicGroup in seismicLine.GroupBy(sl => sl.LayerName))
                    surfaceApi.Values[seismicGroup.Key] = new();

                foreach (var seismicGroup in isVertical ? seismicLine.GroupBy(sl => sl.Y) : seismicLine.GroupBy(sl => sl.X))
                {
                    surfaceApi.Position.Add(seismicGroup.Key);
                    foreach (var seismic in seismicGroup)
                        surfaceApi.Values[seismic.LayerName].Add(seismic.Z);
                }

                arraySurfaceApi.Add(surfaceApi);
            }

            return Ok(arraySurfaceApi);
        }
    }
}