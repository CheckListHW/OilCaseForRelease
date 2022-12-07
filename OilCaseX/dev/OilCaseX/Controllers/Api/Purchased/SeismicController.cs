using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModels = OilCaseApi.Models;
using ApiModels = OilCaseApi.Controllers.ApiModels;

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/Purchased/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class SeismicController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public SeismicController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string userName)
            => _context.Users.Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// Возвращает купленные сейсмики
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApiModels.PurchasedSeismic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var purchasedSeismic = _context.PurchasedSeismic
                .Where(ps => ps.TeamId == team.Id)
                .Select(ps => ApiModels.PurchasedSeismic.FromPurchasedSeismic(ps));
            return Ok(purchasedSeismic);
        }


        /// <summary>
        /// Добавляет купленный объект
        /// </summary>
        /// <param name="value"></param>
        /// <response code="200">Id нового объекта</response>
        /// <response code="409">Уже есть купленная сейсмика с данными координатами</response>
        /// <returns>Id нового объекта</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Post([FromBody] ApiModels.PurchasedSeismic value)
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            DbModels.PurchasedSeismic purchasedSeismic = new()
            {
                TeamId = team.Id,
                GameStep = team.GameStep,
                StartCellX = value.StartCellX,
                StartCellY = value.StartCellY,
                EndCellX = value.EndCellX,
                EndCellY = value.EndCellY
            };
            if (_context.PurchasedSeismic
                    .FirstOrDefault(ps => ps.TeamId == purchasedSeismic.TeamId
                                          & ps.StartCellX == purchasedSeismic.StartCellX
                                          & ps.StartCellY == purchasedSeismic.StartCellY
                                          & ps.EndCellX == purchasedSeismic.EndCellX
                                          & ps.EndCellY == purchasedSeismic.EndCellY) != null) 
                return Conflict();
            _context.PurchasedSeismic.Add(purchasedSeismic);
            _context.SaveChanges();
            return Ok(purchasedSeismic.Id);
        }

        /// <summary>
        /// Удаление сейсмики
        /// </summary>
        /// <param name="id"></param>
        /// <response code="409">Указаный Id прнеадлежит другой команде или шаг покупки уже завершен</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Delete(int id)
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var value = _context.PurchasedSeismic.Find(id);
            if (value?.TeamId != team.Id) return Conflict();
            if (value.GameStep != team.GameStep) return Conflict("Неверный шаг завершения покупи");

            _context.PurchasedSeismic.Remove(value);
            _context.SaveChanges();
            return Ok();
        }
    }
}