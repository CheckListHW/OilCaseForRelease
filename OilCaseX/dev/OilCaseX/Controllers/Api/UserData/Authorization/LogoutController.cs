using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DbModels = OilCaseApi.Models;

namespace OilCaseApi.Controllers.Api.UserData.Authorization
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LogoutController : ControllerBase
    {
        private readonly SignInManager<DbModels.User> _signInManager;

        public LogoutController(SignInManager<DbModels.User> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// [Future] Пока в токен не обнуляется
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
