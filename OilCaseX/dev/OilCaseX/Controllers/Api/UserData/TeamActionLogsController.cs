using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilCaseApi.Utils;
using DbModels = OilCaseApi.Models;
using ApiModels = OilCaseApi.Controllers.ApiModels;

namespace OilCaseApi.Controllers.Api.UserData
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class TeamActionLogsController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public TeamActionLogsController(DbModels.ApplicationContext context)
        {
            _context = context;
        }
        protected DbModels.User? GetUser(string username) => _context.Users.Include(u => u.Team).FirstOrDefault(u => u.UserName == username);

        /// <summary>
        /// Действие и описание
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] ApiModels.TeamAction query)
        {
            var user = GetUser(User.Claims.FirstOrDefault()?.Value);
            if (user == null)
            {
                return Unauthorized();
            };

            _context.TeamActionLogs.Add(new DbModels.TeamActionLogs()
            {
                User = user,
                Description = query.Description,
                TypeAction = query.TypeAction,
                Time = TimeProject.TimeNow()
            });

            _context.SaveChanges();
            return Ok();
        }
    }
}
