using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OilCaseApi.Controllers.ApiModels;
using DbModels = OilCaseApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.LithologicalData
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class LithologicalModelController : ControllerBase
    {
        public DbModels.ApplicationContext _context { get; set; }

        public LithologicalModelController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string userName)
            => _context.Users
                .Include(u => u.Team)
                .ThenInclude(t => t.LithologicalModel)
                .FirstOrDefault(u => u.UserName == userName);

        /// <summary>
        /// Возвращает информацию о текущей литологической модели 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(LithologicalModelInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public object Get()
        {
            var user = GetUser(User.Claims.FirstOrDefault().Value);
            if (user == null) return Unauthorized();

            var lm = user.Team?.LithologicalModel;
            LithologicalModelInfo l = new()
            {
                MinX = lm.MinX,
                MaxX = lm.MaxX,
                MinY = lm.MinY,
                MaxY = lm.MaxY,
                MinZ = lm.MinZ,
                MaxZ = lm.MaxZ,
                Information = lm.Information,
                Name = lm.Name
            };
            return Ok(l);
        }
    }
}