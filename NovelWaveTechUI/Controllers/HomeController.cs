using Application.ApiHttpClient;
using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NovelWaveTechUI.BaseURL;
using NovelWaveTechUI.Filters;
using NovelWaveTechUI.Models;
using System;
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
        private readonly IHttpClients _httpClient;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,IHttpClients httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient=httpClient;
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

            var response = await _httpClient.GetAsync(fullUrl,jwtToken).ConfigureAwait(false);
            return Ok(response);
        }
        public IActionResult Index()
        {
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
            var response = await _httpClient.PostAsync(fullUrl, registerDTO).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.PostAsync(fullUrl, loginDTO).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.GetAsync(fullUrl, jwtToken).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.PostAsync(fullUrl, changePasswordDTO,jwtToken).ConfigureAwait(false);
            return Ok(response);
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Chat()
        {
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
            var response = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.GetAsync(fullUrl, jwtToken).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.GetAsync(fullUrl, jwtToken).ConfigureAwait(false);
            return Ok(response);
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
            var response = await _httpClient.PostAsync(fullUrl, userEditDTO, jwtToken).ConfigureAwait(false);
            return Ok(response);
        }
        
    }
}
