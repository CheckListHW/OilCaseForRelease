using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DbModels = OilCaseApi.Models;
using ApiModels = OilCaseApi.Controllers.ApiModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/Purchased/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class MapController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public MapController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModels.User? GetUser(string userName)
            => _context.Users.Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);

        [HttpGet]
        public IActionResult Get()
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var purchasedMaps = _context.PurchasedMaps
                .Where(ps => ps.TeamId == team.Id)
                .Select(ps => new ApiModels.PurchasedMap()
                {
                    Id = ps.Id,
                    Name = ps.Name,
                });
            return Ok(purchasedMaps);
        }


       
        [HttpPost("{mapName}")]
        public IActionResult Post(string mapName)
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            if (_context.PurchasedMaps.Any(pm => pm.TeamId == team.Id & pm.Name == mapName))
                return Conflict("Already exist");
            var newMap = new DbModels.PurchasedMap()
            {
                Name = mapName,
                GameStep = team.GameStep,
                Team = team
            };

            _context.PurchasedMaps.Add(newMap);
            _context.SaveChanges();
            return Ok(newMap.Id);
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var team = GetUser(User.Claims.FirstOrDefault().Value)?.Team;
            if (team == null) return Unauthorized();

            var value = _context.PurchasedMaps.Find(id);
            if (value?.TeamId != team.Id) return Conflict();

            _context.PurchasedMaps.Remove(value);
            _context.SaveChanges();
            return Ok();
        }
    }
}