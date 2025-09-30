using Application.ApiHttpClient;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NovelWaveTechUI.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClients _httpClient;

        public CustomersController(IConfiguration configuration, IHttpClients httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/customers";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<List<CustomerDataViewModel>>(response);
                return View(product);
            }
        }
    }
}
