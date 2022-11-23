using Microsoft.AspNetCore.Mvc;
using MigDashboard.Models;
using MigDashboard.Services;
using System.Diagnostics;

namespace MigDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string? _scriptLogFilePath { get; }  // python脚本日志文件路径

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _scriptLogFilePath = configuration.GetSection("ScriptLog").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            if (_scriptLogFilePath == null) 
                return View(new List<List<string>>());
            return View(UtilityService.LogFileContent(_scriptLogFilePath));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}