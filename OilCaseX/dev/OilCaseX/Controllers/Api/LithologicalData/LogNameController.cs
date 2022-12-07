using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModel = OilCaseApi.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.LithologicalData
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]

    public class LogNameController : ControllerBase
    {
        private readonly DbModel.ApplicationContext _context;

        public LogNameController(DbModel.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModel.User? GetUser(string userName)
            => _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);


        /// <summary>
        /// Возвращает информацию о кривых (имя, описание, стоимость в днях, стоимость в валюте, группу)
        /// </summary>
        /// <response code="200">Возвращает информацию</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApiModels.LogName>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModel.User? user;
            if ((user = GetUser(User.Claims.FirstOrDefault().Value)) == null)
                return Unauthorized();

            var team = user.Team;
            var result = _context.LogNames.Where(ln => ln.LithologicalModelId == team.LithologicalModelId)
                .Select(ln => new ApiModels.LogName()
                {
                    Name = ln.Name,
                    Description = ln.Description,
                    CostMoney = ln.CostMoney,
                    CostDay = ln.CostDay,
                    Group = ln.Group,
                }).ToArray();

            return Ok(result);
        }
    }
}