using System.IO.Compression;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiModels = OilCaseApi.Controllers.ApiModels;
using DbModels = OilCaseApi.Models;
using OilCaseApi.resources;

namespace OilCaseApi.Controllers.Api.LithologicalData
{
    [Route("api/v1/LithologicalData/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [EnableCors]
    [Produces("application/json")]
    public class BoreholeController : ControllerBase
    {
        private readonly DbModels.ApplicationContext _context;

        public BoreholeController(DbModels.ApplicationContext context)
        {
            _context = context;
        }

        protected double GetRandomNumber(double minimum, double maximum) =>
            (new Random()).NextDouble() * (maximum - minimum) + minimum;

        protected string? CorePhotoDirectory(int modelId, int faciaType)
        {
            string modelDir = $"Data\\CorePhoto\\Model_{modelId}";
            DirectoryInfo[] files = new DirectoryInfo(modelDir).GetDirectories();
            return faciaType < files.Length
                ? files[faciaType].FullName
                : files.Length > 0
                    ? files[faciaType].FullName
                    : null;
        }

        protected string GetRandomFile(string dirPath) => (new DirectoryInfo(dirPath).GetFiles())
            .OrderBy(x => (new Random()).NextDouble())
            .ToArray()[0].Name;

        protected IActionResult GetZipFolder(int modelId, int x, int y)
        {
            var wellLogs = _context.Cells
                .Include(c => c.BoreholeLogs)
                !.ThenInclude(bl => bl.LogName)
                .FirstOrDefault(c => c.X == x & c.Y == y & c.LithologicalModelId == modelId)
                ?.BoreholeLogs
                .OrderBy(wl => wl.Z)
                .Where(wl => wl.LogName.Name == RequiredColumnNames.Facies)
                .ToArray() ?? Array.Empty<DbModels.BoreholeLog>();

            if (!wellLogs.Any())
                return NoContent();

            double lastFacia = wellLogs[0].Value, previousDeep = wellLogs[0].Z;
            var kernPhotos = new List<CorePhoto>();

            foreach (var mc in wellLogs.Where(wc => (int)lastFacia != (int)wc.Value))
            {
                kernPhotos.Add(new CorePhoto()
                {
                    Num = kernPhotos.Count,
                    Deep = GetRandomNumber(previousDeep, mc.Z),
                    FolderPhoto = CorePhotoDirectory(modelId, (int)(lastFacia = mc.Value))
                });
                previousDeep = mc.Z;
            }

            kernPhotos.Add(new CorePhoto()
            {
                Num = kernPhotos.Count,
                Deep = GetRandomNumber(previousDeep, wellLogs.Last().Z),
                FolderPhoto = CorePhotoDirectory(modelId, (int)lastFacia)
            });

            var zipStream = new MemoryStream();
            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var oneItem in kernPhotos)
                    zip.CreateEntryFromFile($@"{oneItem.FolderPhoto}/{GetRandomFile(oneItem.FolderPhoto)}",
                        $@"x{x}_y{y}_{Math.Abs((int)oneItem.Deep)}_{oneItem.Num + 1}.jpg");
            }


            return File(zipStream.ToArray(), MediaTypeNames.Application.Zip, $"Core_x{x}_y{y}.zip");
        }

        /// <summary>
        /// Возвращает zip архив c текстовыми данными и фотографиями
        /// </summary>
        /// <param name="x"> Координата сквжины  по X</param>
        /// <param name="y"> Координата сквжины  по Y</param>
        /// <returns>Zip file with *.txt files and photos</returns>
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [HttpGet("{x}/{y}")]
        public IActionResult Get(int x, int y)
        {
            DbModels.User? user = _context.Users.Include(u => u.Team).FirstOrDefault(u => u.UserName == User.Identity.Name);

            int? modelId = user.Team.LithologicalModelId!;
            
            try
            {
                return modelId == null ? NotFound() : GetZipFolder((int)modelId, x, y);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        private class CorePhoto
        {
            public double Deep { get; set; }
            public int Num { get; set; }
            public string? FolderPhoto { get; set; }
        }
    }
}