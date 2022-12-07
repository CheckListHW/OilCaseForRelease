using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbModel = OilCaseApi.Models;
using OilCaseApi.resources;

namespace OilCaseApi.Controllers.Api.Purchased
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    public class ObjectOfArrangementController : ControllerBase
    {
        private readonly DbModel.ApplicationContext _context;

        public ObjectOfArrangementController(DbModel.ApplicationContext context)
        {
            _context = context;
        }

        protected DbModel.User? GetUser(string? userName)
            => _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.UserName == userName);
        
        /// <summary>
        /// Возвращает карту купленных объектов обустройства
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ApiModels.PurchasedObjectOfArrangementGet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            DbModel.Team? team;
            if ((team = GetUser(User.Claims.FirstOrDefault().Value)?.Team) == null)
                return Unauthorized();

            return Ok(_context.PurchasedObject
                .Include(p => p.ObjectOfArrangement)
                .Where(po => po.Team == team)
                .Select(po => new ApiModels.PurchasedObjectOfArrangementGet()
                {
                    Id = po.Id,
                    Key = po.ObjectOfArrangement.Key,
                    CellX = po.CellX,
                    CellY = po.CellY,
                    SubCellX = po.SubCellX,
                    SubCellY = po.SubCellY,
                    GameStep = po.GameStep
                }));
        }


        /// <summary>
        /// Удаляет купленные объекты обустройства если они были купленны на текущем ходу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            DbModel.Team? team;
            if ((team = GetUser(User.Claims.FirstOrDefault()?.Value)?.Team) == null) return Unauthorized();

            var objectOfArrangement = _context.PurchasedObject.Find(id);
            if (objectOfArrangement?.TeamId != team.Id) return NotFound();
            if (objectOfArrangement.GameStep != team.GameStep)
                return Conflict($"Объект был куплен на шаге {objectOfArrangement.GameStep}, а текущий шаг {team.GameStep}");

            _context.PurchasedObject.Remove(objectOfArrangement);
            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Добавляет в купленные объекты обустройства
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public IActionResult Post([FromBody] ApiModels.PurchasedObjectOfArrangement request)
        {
            DbModel.Team? team;
            if ((team = GetUser(User.Claims.FirstOrDefault()?.Value)?.Team) == null)
                return Unauthorized();

            var objectsOfArrangement = _context.ObjectsOfArrangement.ToArray();
            var currentObject = objectsOfArrangement.FirstOrDefault(oa => oa.Key == request.Key);
            if (currentObject == null)
                return Conflict($"объекта с ключом: ({request.Key}) не существует.");

            var currentPurchasedObjects = _context.PurchasedObject
                .Include(p => p.ObjectOfArrangement)
                .FirstOrDefault(po =>
                    po.TeamId == team.Id
                    & po.CellX == request.CellX
                    & po.CellY == request.CellY
                    & po.SubCellX == request.SubCellX
                    & po.SubCellY == request.SubCellY);

            if (currentPurchasedObjects != null)
                return Conflict(
                    $"В текущем месте уже есть объект ({currentPurchasedObjects.Id}):  {currentPurchasedObjects?.ObjectOfArrangement?.Key}");

            _context.PurchasedObject.Add(new ()
            {
                TeamId = team.Id,
                GameStep = team.GameStep,
                CellX = request.CellX,
                CellY = request.CellY,
                SubCellX = request.SubCellX,
                SubCellY = request.SubCellY,
                ObjectOfArrangement = currentObject
            });
            _context.SaveChanges();

            return Ok();
        }
    }
}