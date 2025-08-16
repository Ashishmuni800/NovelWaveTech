using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
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
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> UserlistApi(int skip, int take)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Get?skip={skip}&take={take}";

            var response = await _httpClient.GetAsync(fullUrl,true).ConfigureAwait(false);
            return Ok(response);
        }
        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
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
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> LoginPost([FromBody] LoginDTO loginDTO)
        {

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Login";
            var response = await _httpClient.PostAsync(fullUrl, loginDTO).ConfigureAwait(false);
            var tokenObj = JsonConvert.DeserializeObject<TokenResponse>(response);
            var token = tokenObj?.Token;

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token received.");
            }
            var copkiesOtions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(15)
            };
            Response.Cookies.Append("AuthToken", token, copkiesOtions);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult Proctected()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> weatherForecast()
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/weatherForecast";
            var response = await _httpClient.GetAsync(fullUrl, true).ConfigureAwait(false);
            return Ok(response);
        }
        public IActionResult PasswordChange()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChangePost([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/ChangePassword";
            var response = await _httpClient.PostAsync(fullUrl, changePasswordDTO,true).ConfigureAwait(false);
            return Ok(response);
        }
        public IActionResult Profile()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        public IActionResult Chat()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
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
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return Unauthorized();
            
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetToken?token={token}";
        
            var response = await _httpClient.GetAsync(fullUrl, true).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response)) return Unauthorized();
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Delete/{Id}";
            var response = await _httpClient.GetAsync(fullUrl, true).ConfigureAwait(false);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] UserEditDTO userEditDTO, [FromRoute] string Id)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/EditUser/{Id}";
            var response = await _httpClient.PostAsync(fullUrl, userEditDTO, true).ConfigureAwait(false);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/Logout";
            var response = await _httpClient.GetAsync(fullUrl, true).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response)) return Unauthorized();
            Response.Cookies.Delete("AuthToken");
            return Ok(response);
        }
    }
}
