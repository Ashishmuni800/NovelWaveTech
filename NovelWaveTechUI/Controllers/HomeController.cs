using NovelWaveTechUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace NovelWaveTechUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Proctected()
        {
            return View();
        }
        public IActionResult PasswordChange()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
