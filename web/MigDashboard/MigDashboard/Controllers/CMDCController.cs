using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Net.Mime;

namespace MigDashboard.Controllers
{
    public class CMDCController : Controller
    {
        private readonly IFileProvider _waitingFileProvider;
        private readonly IFileProvider _completedFileProvider;       
        public CMDCController(IConfiguration configuration) 
        {           
            _waitingFileProvider = new PhysicalFileProvider(
                configuration["CMDC:PonProcessor:WaitingFolder"]);
            _completedFileProvider = new PhysicalFileProvider(
                configuration["CMDC:PonProcessor:CompletedFolder"]);
        }
        public IActionResult Index()
        {            
            return View();
        }
        public IActionResult PonProcessor()
        {
            ViewData["waitingFiles"] = _waitingFileProvider.GetDirectoryContents(string.Empty);
            ViewData["completedFiles"] = _completedFileProvider.GetDirectoryContents(string.Empty);
            return View();
        }

        public IActionResult DownCompletedFile(string fileName)
        {
            var downloadFile = _completedFileProvider.GetFileInfo(fileName);

            return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, fileName);
        }
    }
}
