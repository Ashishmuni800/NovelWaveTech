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
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Userlist()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Index()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Register()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Login()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        [HttpGet]
        public IActionResult Proctected()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult PasswordChange()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Profile()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Chat()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
    }
}
