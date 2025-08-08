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
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Userlist()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UserlistApi(int skip, int take)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Get?skip={skip}&take={take}";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }
        public IActionResult Index()
        {
            string baseurl = _configuration["BaseUrl"];
            ViewBag.URLs = baseurl;
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterPost([FromBody] RegisterDTO registerDTO)
        {

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Register";

            using var client = new HttpClient();

            var jsonContent = JsonConvert.SerializeObject(registerDTO);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(fullUrl, httpContent);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
            var responseString = await response.Content.ReadAsStringAsync();

            return Content(responseString, contentType);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginPost([FromBody] LoginDTO loginDTO)
        {

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Login";

            using var client = new HttpClient();

            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(fullUrl, httpContent);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
            var responseString = await response.Content.ReadAsStringAsync();

            return Content(responseString, contentType);
        }

        [HttpGet]
        public IActionResult Proctected()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> weatherForecast()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/weatherForecast";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }
        public IActionResult PasswordChange()
        {
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
        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetToken/{jwtToken}";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Delete/{Id}";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] UserEditDTO userEditDTO, [FromRoute] string Id)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header.");

            string jwtToken = authHeader.Substring("Bearer ".Length).Trim();

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/EditUser/{Id}";

            using var client = new HttpClient();

            var jsonContent = JsonConvert.SerializeObject(userEditDTO);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.PostAsync(fullUrl, httpContent);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            return Ok(contentStream);
        }
        
    }
}
