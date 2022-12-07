using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModels = OilCaseApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.UserData
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class CompleteMoveController : ControllerBase
    {

        private readonly DbModels.ApplicationContext _context;
        public CompleteMoveController(DbModels.ApplicationContext context)
        {
            _context = context;
        }
        protected DbModels.User? GetUser(string username) => _context.Users.Include(u => u.Team).FirstOrDefault(u => u.UserName == username);

        /// <summary>
        /// Посылает сигнал о завершении хода команды
        /// </summary>
        /// <response code="200">Gamestep += 1</response>
        [HttpPost]
        public IActionResult Post()
        {
            DbModels.User? user = GetUser(User.Claims.FirstOrDefault().Value);
            if (user == null) {
                return Unauthorized();
            };
            user.Team.GameStep += 1;
            _context.Update(user.Team);
            _context.SaveChanges();
            return Ok("Success");
        }
    }
}
