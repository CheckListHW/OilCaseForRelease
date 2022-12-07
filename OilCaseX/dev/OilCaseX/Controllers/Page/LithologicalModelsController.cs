using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilCaseApi.Models;
using FileControl = System.IO;

namespace OilCaseApi.Controllers
{
    [Authorize(Roles = StandardRoles.Admin)]
    public class LithologicalModelsController : Controller
    {
        private readonly ApplicationContext _context;

        public LithologicalModelsController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Debug.WriteLine(User.Identity.Name);
            return _context.LithologicalModel != null
                ? View(await _context.LithologicalModel.ToListAsync())
                : Problem("Entity set 'ApplicationContext.LithologicalModel'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LithologicalModel == null)
            {
                return NotFound();
            }

            var lithologicalModel = await _context.LithologicalModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lithologicalModel == null)
            {
                return NotFound();
            }

            return View(lithologicalModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] LithologicalModel lithologicalModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lithologicalModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lithologicalModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LithologicalModel == null)
            {
                return NotFound();
            }

            var lithologicalModel = await _context.LithologicalModel.FindAsync(id);
            if (lithologicalModel == null)
            {
                return NotFound();
            }

            return View(lithologicalModel);
        }

        [HttpPost]
        [DisableRequestSizeLimit,
         RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Edit(int id, string name, IFormFile? UploadFileBoreholeLogs, IFormFile? UploadFileSeismic)
        {
            var lithologicalModel = _context.LithologicalModel.FirstOrDefault(x => x.Id == id);
            if (lithologicalModel == null)
                return NotFound();

            if (string.IsNullOrEmpty(name))
                name = lithologicalModel.Name;

            if (_context.LithologicalModel.FirstOrDefault(x => x.Name == name) == null)
            {
                lithologicalModel.Name = name;
                _context.Update(lithologicalModel);
                await _context.SaveChangesAsync();
            }

            string? message = "";
            if (UploadFileBoreholeLogs?.FileName != null)
                message = await LoadLithologicalDataFromFile(id, UploadFileBoreholeLogs, lithologicalModel.LoadBoreholeLogsFromCsv)!;

            if (UploadFileSeismic?.FileName != null)
                message += await LoadLithologicalDataFromFile(id, UploadFileSeismic, lithologicalModel.LoadSeismicFromCsv)!;

            Debug.WriteLine($"message {message}");

            return string.IsNullOrEmpty(message)
                ? RedirectToAction("Details", new { id })
                : Content(message);
        }

        public IActionResult DeleteGisAndSeismic(int id)
        {
            _context.LithologicalModel.FirstOrDefault(lm => lm.Id == id)?.DeleteAll();
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> DeleteDuplicateGis(int id)
        {
            (await _context.LithologicalModel.FindAsync(id))?.DeleteDuplicateRows();
            return RedirectToAction("Details", new { id });
        }


        public (string, string) SaveFileOnDisc(IFormFile? uploadedFile, string uploadPath)
        {
            Directory.CreateDirectory(uploadPath);

            List<double> foldersName = new() { 0.0 };
            foldersName.AddRange(Directory.GetDirectories(uploadPath)
                .Select(x => x.Split("\\").Last())
                .Where(x => double.TryParse(x, out _))
                .Select(Convert.ToDouble));

            long folderNumber = 1 + (int)foldersName.Max();

            var currentFileDirectory = $"{uploadPath}\\{folderNumber}";
            Directory.CreateDirectory(currentFileDirectory);

            var fullPath = $"{currentFileDirectory}\\{uploadedFile.FileName}";

            using var fileStream = new FileStream(fullPath, FileMode.Create);
            uploadedFile.CopyTo(fileStream);
            fileStream.Close();

            return (currentFileDirectory, uploadedFile.FileName);
        }

        private static void DeleteTempFile(string directoryPath)
        {
            foreach (var filePath in Directory.GetFiles($"{directoryPath}"))
                try
                {
                    using var fs = FileControl.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    fs.Close();
                    FileControl.File.Delete(filePath);
                }
                catch (IOException ioex)
                {
                    Debug.WriteLine("файл занят");
                }

            if (Directory.GetFileSystemEntries(directoryPath).Length == 0)
                Directory.Delete(directoryPath, true);
        }


        //public async Task<string?>? LoadBoreholeLogsFile(int lithologicalModelId, IFormFile? uploadedFile)
        //{
        //    if (uploadedFile == null)
        //        return "no content";

        //    var uploadPath = $"{Directory.GetCurrentDirectory()}\\uploads";
        //    var (directoryPath, fileName) = SaveFileOnDisc(uploadedFile, uploadPath);
        //    var lithological = _context.LithologicalModel
        //        .FirstOrDefault(x => x.Id == lithologicalModelId);

        //    string? message = null;
        //    if (lithological != null)
        //        message = lithological.LoadBoreholeLogsFromCsv($"{directoryPath}\\{fileName}");

        //    foreach (string path in Directory.GetDirectories(uploadPath))
        //        DeleteTempFile(path);
        //    return message;
        //}

        public async Task<string?>? LoadLithologicalDataFromFile(int lithologicalModelId, IFormFile? uploadedFile, 
            Func<string, string> loadDataFromCsv)
        {
            if (uploadedFile == null)
                return "no content";

            var uploadPath = $"{Directory.GetCurrentDirectory()}\\uploads";
            var (directoryPath, fileName) = SaveFileOnDisc(uploadedFile, uploadPath);
            var message = loadDataFromCsv($"{directoryPath}\\{fileName}");

            foreach (string path in Directory.GetDirectories(uploadPath))
                DeleteTempFile(path);
            return message;
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LithologicalModel == null)
            {
                return NotFound();
            }

            var lithologicalModel = await _context.LithologicalModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lithologicalModel == null)
            {
                return NotFound();
            }

            return View(lithologicalModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LithologicalModel == null)
            {
                return Problem("Entity set 'ApplicationContext.LithologicalModel'  is null.");
            }

            var lithologicalModel = await _context.LithologicalModel.FindAsync(id);
            if (lithologicalModel != null)
            {
                _context.LithologicalModel.Remove(lithologicalModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}