using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NovelWaveTechUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClients _httpClient;

        public AdminController(IConfiguration configuration, IHttpClients httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetRoles";

            // Call API
            var response = await _httpClient.GetAsync(fullUrl);
            var tokenObj = JsonConvert.DeserializeObject<RolesResponse>(response);
            var roles = tokenObj?.Roles ?? new List<string>();

            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else if (roles.Contains("Admin"))
            {
                try
                {
                    //string baseUrl = _configuration["BaseUrl"];
                    string fullUrls = $"{baseUrl}/api/Admin/Index";

                    // Send request with streaming enabled
                    var responses = await _httpClient.GetAsync(fullUrls, true);
                    var users = !string.IsNullOrEmpty(responses) ? JsonConvert.DeserializeObject<List<UserWithRolesViewModel>>(responses) : null;

                    return View(users ?? new List<UserWithRolesViewModel>());
                }
                catch (HttpRequestException ex)
                {
                    // Log exception or show an error message
                    ViewBag.Error = "Failed to fetch user list.";
                    return View(new List<UserWithRolesViewModel>());
                }
            }
            else
            {
                return RedirectToAction("Index","Home"); // fallback for non-admin
            }
        }


        public async Task<IActionResult> ManageRoles(string userId)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetRoles";

            // Call API
            var response = await _httpClient.GetAsync(fullUrl);
            var tokenObj = JsonConvert.DeserializeObject<RolesResponse>(response);
            var roles = tokenObj?.Roles ?? new List<string>();
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else if (roles.Contains("Admin"))
            {
                try
                {
                    //string baseUrl = _configuration["BaseUrl"];
                    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // ✅ 1. Get user + assigned roles
                    string userUrl = $"{baseUrl}/api/Admin/ManageRoles?userId={userId}";
                    var userResponse = await _httpClient.GetAsync(userUrl, true);
                    //userResponse.EnsureSuccessStatusCode();
                    //var userJson = await userResponse.Content.ReadAsStringAsync();
                    var userWithRoles = JsonConvert.DeserializeObject<ManageRolesViewModel>(userResponse);

                    // ✅ 2. Get all available roles
                    string rolesUrl = $"{baseUrl}/api/Admin/GetAllRoles";
                    var rolesResponse = await _httpClient.GetAsync(rolesUrl, true);
                    var allRoles = JsonConvert.DeserializeObject<List<string>>(rolesResponse);

                    // ✅ 3. Build view model
                    var model = new ManageRolesViewModel
                    {
                        UserId = userWithRoles.UserId,
                        Email = userWithRoles.Email,
                        AssignedRoles = userWithRoles.AssignedRoles ?? new List<string>(),
                        AllRoles = allRoles ?? new List<string>()
                    };

                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Failed to load role management data.";
                    return View(model: null);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home"); // fallback for non-admin
            }

            
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRoles(ManageRolesDTO model)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetRoles";

            // Call API
            var response = await _httpClient.GetAsync(fullUrl);
            var tokenObj = JsonConvert.DeserializeObject<RolesResponse>(response);
            var roles = tokenObj?.Roles ?? new List<string>();
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else if (roles.Contains("Admin"))
            {
                try
                {
                    //string baseUrl = _configuration["BaseUrl"];
                    string url = $"{baseUrl}/api/Admin/UpdateRoles";

                    var payload = new
                    {
                        userId = model.UserId,
                        roles = model.SelectedRoles
                    };
                    var response2 = await _httpClient.PostAsync(url, payload, true);

                    if (response2.Length > 0)
                    {
                        TempData["SuccessMessage"] = "Roles updated successfully.";
                    }
                    if (response2.Length < 0)
                    {
                        TempData["ErrorMessage"] = "Failed to update roles.";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating roles.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home"); // fallback for non-admin
            }

            
        }
        public async Task<IActionResult> PermissionMatrix(string userId)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetRoles";

            // Call API
            var response = await _httpClient.GetAsync(fullUrl);
            var tokenObj = JsonConvert.DeserializeObject<RolesResponse>(response);
            var roles = tokenObj?.Roles ?? new List<string>();
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else if (roles.Contains("Admin"))
            {
                try
                {
                    //string baseUrl = _configuration["BaseUrl"];
                    string fullUrls = $"{baseUrl}/api/Admin/GetUserPermissions?userId={userId}";

                    // Send request with streaming enabled
                    var response2 = await _httpClient.GetAsync(fullUrls, true);
                    var result = JsonConvert.DeserializeObject<PermissionMatrixViewModel>(response2);

                    // Pass to view
                    return View(result);
                }
                catch (HttpRequestException ex)
                {
                    // Log exception or show an error message
                    ViewBag.Error = "Failed to fetch user list.";
                    return View(new List<PermissionMatrixViewModel>());
                }
            }
            else
            {
                return RedirectToAction("Index", "Home"); // fallback for non-admin
            }

            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserPermissions(PermissionMatrixViewModel request)
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/UserAuth/GetRoles";

            // Call API
            var response = await _httpClient.GetAsync(fullUrl);
            var tokenObj = JsonConvert.DeserializeObject<RolesResponse>(response);
            var roles = tokenObj?.Roles ?? new List<string>();
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else if (roles.Contains("Admin"))
            {
                try
                {
                    //string baseUrl = _configuration["BaseUrl"];
                    string fullUrls = $"{baseUrl}/api/Admin/UpdateUserPermissions";
                    var payload = new UpdatePermissionsRequestDTO()
                    {
                        UserId = request.UserId,
                        Permissions = request.SelectedPermissions,
                    };
                    // Send request with streaming enabled
                    var response2 = await _httpClient.PostAsync(fullUrls, payload, true);
                    if (response2.Length > 0)
                    {
                        TempData["SuccessMessage"] = "Permissios updated successfully.";
                    }
                    if (response2.Length < 0)
                    {
                        TempData["ErrorMessage"] = "Failed to update Permissios.";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating Permissios.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home"); // fallback for non-admin
            }

            
        }
        public async Task<IActionResult> GetUserLoginPermissions()
        {
            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/Admin/GetUserLoginPermissions";
            var response = await _httpClient.GetAsync(fullUrl, true).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response)) return Unauthorized();
            return Ok(response);
        }

        public IActionResult Index2()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public IActionResult NotFounds()
        {
            return View();
        }
        public IActionResult blogGrid()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public IActionResult signin()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }
        }
        public IActionResult signup()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public IActionResult blogSingle()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }
        }
    }
}
