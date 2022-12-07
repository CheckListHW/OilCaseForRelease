using Microsoft.Build.Framework;

namespace OilCaseApi.Controllers.ApiModels
{
    public class Login
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}
