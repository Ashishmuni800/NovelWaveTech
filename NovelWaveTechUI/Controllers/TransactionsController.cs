using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NovelWaveTechUI.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClients _httpClient;

        public TransactionsController(IConfiguration configuration, IHttpClients httpClient)
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
                return View();
            }
        }
        public async Task<IActionResult> CheckBalance(string accountNumber)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (!string.IsNullOrEmpty(accountNumber))
                {
                    string baseUrl = _configuration["BaseUrl"];
                    string fullUrl = $"{baseUrl}/api/customers/{accountNumber}";
                    var response = await _httpClient.GetAsync(fullUrl, true);
                    var customer = JsonConvert.DeserializeObject<CustomerViewModel>(response);
                    if (customer == null) return View();
                    if (!string.IsNullOrEmpty(customer.AccountNumber))
                    {
                        string fullUrls = $"{baseUrl}/api/customers/Transactions/GetBalanceBycustomerId/{customer.Id}";

                        var responses = await _httpClient.GetAsync(fullUrls, true);
                        var product = JsonConvert.DeserializeObject<CustomerBalanceDTO>(responses);
                        //ViewBag.data = product;
                        return View(product);
                    }
                    return View();
                }
                else
                {
                    return View();
                }
            }
        }
        public async Task<IActionResult> AccountStatement(string accountNumber)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (!string.IsNullOrEmpty(accountNumber))
                {
                    string baseUrl = _configuration["BaseUrl"];
                    string fullUrl = $"{baseUrl}/api/customers/{accountNumber}";
                    var response = await _httpClient.GetAsync(fullUrl, true);
                    var customer = JsonConvert.DeserializeObject<CustomerViewModel>(response);

                    if (!string.IsNullOrEmpty(customer?.AccountNumber))
                    {
                        string fullUrls = $"{baseUrl}/api/customers/Transactions/customerId/{customer.Id}";
                        var responses = await _httpClient.GetAsync(fullUrls, true);
                        var product = JsonConvert.DeserializeObject<List<TransactionViewModelDataTable>>(responses);

                        return View(product);
                    }

                    return View();
                }
                else
                {
                    return View();
                }
            }
        }

        public async Task<IActionResult> Index2(string accountNumber)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/customers/{accountNumber}";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var customer = JsonConvert.DeserializeObject<CustomerViewModel>(response);
                if (!string.IsNullOrEmpty(customer.AccountNumber))
                {
                    string fullUrls = $"{baseUrl}/api/customers/Transactions/GetBalanceBycustomerId/{customer.Id}";
                   
                    var responses = await _httpClient.GetAsync(fullUrls, true);
                    var product = JsonConvert.DeserializeObject<CustomerBalanceDTO>(responses);
                    //ViewBag.data = product;
                    return Ok(product);
                }
                return BadRequest("Invalid customer account number");
            }
        }
        public async Task<IActionResult> Get()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/customers/Transactions";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<List<TransactionViewModelDataTable>>(response);
                return Ok(product);
            }
        }
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
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
        public async Task<IActionResult> Create([FromBody] TransactionRequestDTO transactionRequestDTO)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/customers/{transactionRequestDTO.accountNumber}";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var customer = JsonConvert.DeserializeObject<CustomerViewModel>(response);
                if(!string.IsNullOrEmpty(customer.AccountNumber))
                {
                    string fullUrls = $"{baseUrl}/api/customers/Transactions/{customer.AccountNumber}";
                    var TransactionReq = new TransactionRequestDTO
                    {
                        CustomerId = customer.Id,
                        Amount = transactionRequestDTO.Amount,
                        Type = transactionRequestDTO.Type,
                        Notes = transactionRequestDTO.Notes,
                        accountNumber = transactionRequestDTO.accountNumber,
                    };
                    var responses = await _httpClient.PostAsync(fullUrls, TransactionReq, true);
                    //var product = JsonConvert.DeserializeObject<ProductViewModel>(responses);
                    //ViewBag.data = product;
                    return Ok();
                }
                return BadRequest("Invalid customer account number");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
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
                return RedirectToAction("Login", "Home");
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

