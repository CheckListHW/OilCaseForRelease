using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DbModels = OilCaseApi.Models;
using ApiModels = OilCaseApi.Models;
using NuGet.Common;

namespace OilCaseApi.Controllers.Api.UserData.Authorization
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<DbModels.User> _userManager;
        private readonly SignInManager<DbModels.User> _signInManager;
        private readonly DbModels.IJwtGenerator _jwtGenerator;
        public LoginController(UserManager<DbModels.User> userManager,
          SignInManager<DbModels.User> signInManager,
          DbModels.IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        /// <summary>
        /// Get bearer token
        /// </summary>
        /// <response code="200">Returns bearer token</response>
        /// <response code="404">Invalid username or password</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        
        public async Task<IActionResult> Post([FromBody] ApiModels.Login query)
        {
            var user = await _userManager.FindByNameAsync(query.Username);
            if (user == null)
            {
                return NotFound("Invalid username");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, query.Password, false);
            if (result.Succeeded)
            {
                return Ok(_jwtGenerator.CreateToken(user));
            }
            return NotFound("Invalid Password");
        }


    }

    class ResultLogin
    {
        public string team { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }
}