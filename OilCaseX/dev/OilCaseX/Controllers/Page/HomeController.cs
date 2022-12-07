using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OilCaseApi.Models;
using OilCaseApi.resources;
using System.Diagnostics;
using Newtonsoft.Json;

namespace OilCaseApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext _context;


        public HomeController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, ApplicationContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (_roleManager.Roles.Any()) return View();
            
            await DefaultValue.InitBdStartValuesAsync(_context);
            
            foreach (var field in typeof(StandardRoles).GetFields())
                await _roleManager.CreateAsync(new(field.Name));

            var team = _context.Team.First();
            var adminUser = new User()
            {
                Email = "kosach_1@live.com",
                UserName = "kosach_1@live.com",
                TeamId = team.Id,
            };

            await _userManager.CreateAsync(adminUser, "111111");
            await _userManager.AddToRoleAsync(adminUser, StandardRoles.Admin);

            var captainUser = new User()
            {
                Email = "p1@p1.com",
                UserName = "p1@p1.com",
                TeamId = team.Id
            };
            await _userManager.CreateAsync(captainUser, "111111");
            await _userManager.AddToRoleAsync(captainUser, StandardRoles.Captain);


            var testTeam = new Team()
            {
                Name = "TestTeam",
                LithologicalModelId = team.LithologicalModelId,
            };
            _context.Team.Add(testTeam);

            var testCaptainUser = new User()
            {
                Email = "captain@te.st",
                UserName = "captain@te.st",
                Team = testTeam
            };
            await _userManager.CreateAsync(testCaptainUser, "111111");
            await _userManager.AddToRoleAsync(testCaptainUser, StandardRoles.Captain);

            var testRegularUser = new User()
            {
                UserName = "regular@te.st",
                Email = "regular@te.st",
                Team = testTeam
            };
            await _userManager.CreateAsync(testRegularUser, "111111");
            await _userManager.AddToRoleAsync(testRegularUser, StandardRoles.Regular);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}