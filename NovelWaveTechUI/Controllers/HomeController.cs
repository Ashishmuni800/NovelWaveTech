using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovelWaveTechUI.BaseURL;
using NovelWaveTechUI.Filters;
using NovelWaveTechUI.Models;
using System.Diagnostics;

namespace NovelWaveTechUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly BaseURLs _baseURL;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Userlist()
        {
            return View();
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
            //var url = _baseURL.SetURL("/api/UserAuth/GenerateCaptcha");
           // ViewBag.Url = url;
            return View();
        }
        [HttpGet]
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
        public IActionResult Chat()
        {
            return View();
        }
    }
}
