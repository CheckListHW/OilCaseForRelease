using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ApiModels = OilCaseApi.Controllers.ApiModels;
using DbModels = OilCaseApi.Models;

namespace OilCaseApi.Controllers.Api.UserData
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class InfoController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public InfoController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string? username) =>
            _context.Users.Include(u => u.Team).ThenInclude(t => t.LithologicalModel)
                .FirstOrDefault(u => u.UserName == username);

        /// <summary>
        /// Возвращает информацию о пользователе
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiModels.UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            var user = GetUser(User.Claims.FirstOrDefault()?.Value);
            if (user == null)
            {
                return Unauthorized();
            }

            var lm = user.Team?.LithologicalModel;
            var userInfo = new ApiModels.UserInfo()
            {
                TeamInfo = new ApiModels.TeamInfo()
                {
                    Name = user.Team.Name,
                    GameStep = user.Team.GameStep,
                    LithologicalModelInfo = new()
                    {
                        MinX = lm.MinX,
                        MaxX = lm.MaxX,
                        MinY = lm.MinY,
                        MaxY = lm.MaxY,
                        MinZ = lm.MinZ,
                        MaxZ = lm.MaxZ,
                        Information = lm.Information,
                        Name = lm.Name
                    },
                },
                Username = user.UserName,
                Role = _context.Roles.Find(_context.UserRoles
                    .FirstOrDefault(ur => ur.UserId == user.Id)?.RoleId)?.Name,
            };
            return Ok(userInfo);
        }
    }
}