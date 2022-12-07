using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModels = OilCaseApi.Models;
using ApiModels = OilCaseApi.Controllers.ApiModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/Purchased/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class WellTopController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public WellTopController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string? userName)
            => _context.Users
                .Include(u => u.Team)
                .ThenInclude(t => t.PurchasedBoreholeExplorations)!
                .ThenInclude(pb => pb.WellTops)
                .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// Добавление отбивок
        /// </summary>
        /// <param name="value"></param>
        /// <response code="409">Скважина для отбивки не найдена</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Post([FromBody] ApiModels.WellTop value)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault()?.Value)?.Team;
            if (team == null) return Unauthorized();

            var teamPurchasedBorehole = team?.PurchasedBoreholeExplorations?.FirstOrDefault(pb => pb.Id == value.PurchasedBoreholeId);
            if (teamPurchasedBorehole != null)
                return Conflict();

            _context.WellTops.Add(new DbModels.WellTop()
            {
                PurchasedBoreholeId = value.PurchasedBoreholeId,
                Name = value.Name,
                Z = value.Z
            });
            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Удаление отбивок скважин
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Delete(int id)
        {
            DbModels.Team? team = GetUser(User.Claims.FirstOrDefault()?.Value)?.Team;
            if (team == null) return Unauthorized();

            var wellTop = _context.WellTops
                .Include(wt => wt.PurchasedBoreholeExploration)
                .FirstOrDefault(wt => wt.Id == id);
            if (wellTop != null)
                return NotFound();

            if (wellTop.PurchasedBoreholeExploration.GameStep != team.GameStep)
                return Conflict();

            _context.WellTops.Remove(new DbModels.WellTop() { Id = id });
            _context.SaveChanges();
            return Ok();
        }
    }
}