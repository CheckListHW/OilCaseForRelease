using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OilCaseApi.Models;
using OilCaseApi.ViewModels;

namespace OilCaseApi.Controllers
{
    [Authorize(Roles = StandardRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;


        public UsersController(UserManager<User> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var _teams = new Dictionary<int?, Team>();
            foreach (var team in _context.Team.ToList())
            {
                _teams[team.Id] = team;
            }

            Team defaultTeam = new Team() { Name = "---" };

            return View(_userManager.Users.Select(u => new User()
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Team = u.TeamId != null ? _teams[u.TeamId] : defaultTeam,
            }).ToList());
        }

        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, TeamId = model.TeamId };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewData["TeamId"] = new SelectList(_context.Team, "Id", "Name");
            return View(model);
        }


        public IEnumerable<IdentityRole> GetUserRoles(User user)
        {
            var allRoles = _context.Roles;
            var userRoles = _context.UserRoles
                .Where(x => x.UserId == user.Id);

            var userRoleNames = allRoles
                .Where(role => userRoles.FirstOrDefault(userRole => role.Id == userRole.RoleId) != null)
                .ToArray();

            return userRoleNames;
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.Teams = new SelectList(_context.Team, "Id", "Name");
            ViewBag.Roles = _context.Roles;

            var userRoleNames = GetUserRoles(user).ToArray();

            var team = await _context.Team.FindAsync(user.TeamId);

            return View(new EditUserViewModel
            {
                Id = user.Id, Email = user.Email,
                Roles = userRoleNames, Team = team, TeamId = team?.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.TeamId = model.TeamId;
                    
                    var roles = GetUserRoles(user).Select(u => u.Name);
                    var newRoles= model.RoleNames?.Where(r => !roles.Contains(r)).ToArray();
                    if (newRoles != null)
                        await _userManager.AddToRolesAsync(user, newRoles);

                    var unusedRoles = _context.Roles.Select(r => r.Name).Where(r => roles.Contains(r));
                    if (model.RoleNames != null)
                        unusedRoles = unusedRoles.Where(r => !model.RoleNames.Contains(r));

                    if (unusedRoles != null)
                        await _userManager.RemoveFromRolesAsync(user, unusedRoles);

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Teams = new SelectList(_context.Team, "Id", "Name");
            ViewBag.Roles = _context.Roles;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}