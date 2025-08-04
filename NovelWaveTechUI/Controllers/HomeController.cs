using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovelWaveTechUI.BaseURL;
using NovelWaveTechUI.Filters;
using NovelWaveTechUI.Models;
using System.Buffers.Text;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

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
            string baseUrl = _configuration["BaseUrl"];
            string baseurls = $"{baseUrl}/api/UserAuth/GenerateCaptcha";
            ViewBag.URLs = baseurls;
            return View();
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChangePost([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/ChangePassword";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var jsonContent = JsonConvert.SerializeObject(changePasswordDTO);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(fullUrl, httpContent);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
            var responseString = await response.Content.ReadAsStringAsync();

            return Content(responseString, contentType);
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

        [HttpGet]
        public async Task<IActionResult> Captcha()
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GenerateCaptcha";

            using var client = new HttpClient();
            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var content = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/png";
            var contentDisposition = response.Content.Headers.ContentDisposition?.ToString()
                                     ?? $"inline; filename={Guid.NewGuid()}";

            Response.Headers["Content-Disposition"] = contentDisposition;

            return File(content, contentType);
        }
        [HttpGet]
        public async Task<IActionResult> GetCaptcha( string CaptchaCode)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetCaptcha/{CaptchaCode}";

            using var client = new HttpClient();
            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }



    }
}
