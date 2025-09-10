using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace NovelWaveTechUI.Controllers
{
    public class ProductUIController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClients _httpClient;

        public ProductUIController(IConfiguration configuration, IHttpClients httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/GetProducts";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<List<ProductViewModelData>>(response);
                //ViewBag.data = product;
                return View(product);
            }
        }
        public async Task<IActionResult> GetByUserId()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/GetProductByUserId";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response);
                //ViewBag.data = product;
                return View(product);
            }
        }
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/DeleteProductById?Id={Id}";
                var response = await _httpClient.GetAsync(fullUrl, true);
                //var product = JsonConvert.DeserializeObject<ProductViewModel>(response);
                //ViewBag.data = product;
                return Ok(response);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product2DTO productDTO)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/CreateProduct";
                var response = await _httpClient.PostAsync(fullUrl, productDTO, true);
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response);
                //ViewBag.data = product;
                return Ok(product);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/GetProductById?Id={Id}";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response);
                //ViewBag.data = product;
                return Ok(product);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Product2DTO productDTO)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/EditProduct";

                var response = await _httpClient.PostAsync(fullUrl, productDTO, true);
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response);
                //ViewBag.data = product;
                return Ok(product);
            }
        }
    }
}
