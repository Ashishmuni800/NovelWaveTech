using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using System.Text;

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

        public async Task<IActionResult> DownloadPdf(string accountNumber)
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
                        var products = JsonConvert.DeserializeObject<List<TransactionViewModelDataTable>>(responses);

                        var doc = QuestPDF.Fluent.Document.Create(container =>
                        {
                            container.Page(page =>
                            {
                                page.Margin(20);

                                // Header
                                page.Header().PaddingBottom(15)
                                    .AlignCenter()
                                    .Text("Account Statement")
                                    .FontSize(18)
                                    .Underline()
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                                // Content
                                page.Content().Table(table =>
                                {
                                    // Define columns
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(1);    // Amount
                                        c.RelativeColumn(1.5f); // Type
                                        c.RelativeColumn(2);    // Notes
                                        c.RelativeColumn(1.5f); // Transaction Date
                                    });

                                    // Header row
                                    table.Header(h =>
                                    {
                                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Amount").Bold();
                                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Type").Bold();
                                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Notes").Bold();
                                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Transaction Date").Bold();
                                    });

                                    // Data rows with zebra striping
                                    for (int i = 0; i < products.Count; i++)
                                    {
                                        var item = products[i];
                                        var bgColor = (i % 2 == 0) ? QuestPDF.Helpers.Colors.White : QuestPDF.Helpers.Colors.Grey.Lighten4;

                                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Amount.ToString("C2")); // formatted as currency
                                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Type ?? "");
                                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Notes ?? "");
                                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(
                                            string.IsNullOrEmpty(item.TransactionDate) ? "" : DateTime.Parse(item.TransactionDate).ToString("dd-MM-yyyy")
                                        );
                                    }
                                });

                                // Footer with date and page number
                                page.Footer()
                                    .AlignCenter()
                                    .Row(row =>
                                    {
                                        row.RelativeColumn().AlignLeft().Text($"Generated on: {DateTime.Now:dd-MM-yyyy HH:mm}");
                                        row.RelativeColumn().AlignRight().Text(txt =>
                                        {
                                            txt.CurrentPageNumber();
                                            txt.Span(" / ");
                                            txt.TotalPages();
                                        });
                                    });
                            });
                        });

                        // Generate PDF and return as file
                        //var pdf = doc.GeneratePdf();
                        //return File(pdf, "application/pdf", "AccountStatement.pdf");

                        var pdfBytes = doc.GeneratePdf();
                        string ownerPassword = "MySecureAdminPassword"; // your admin password

                        using var inputStream = new MemoryStream(pdfBytes);
                        using var outputStream = new MemoryStream();

                        using (var pdfReader = new PdfReader(inputStream))
                        {
                            var writerProps = new WriterProperties()
                                .SetStandardEncryption(
                                    Encoding.UTF8.GetBytes(accountNumber), // user password
                                    Encoding.UTF8.GetBytes(ownerPassword), // owner password
                                    EncryptionConstants.ALLOW_PRINTING,    // ✅ only allow printing
                                    EncryptionConstants.ENCRYPTION_AES_256
                                );

                            using var pdfWriter = new PdfWriter(outputStream, writerProps);
                            using var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
                            pdfDoc.Close();
                        }

                        var protectedPdf = outputStream.ToArray();
                        return File(protectedPdf, "application/pdf", "AccountStatement.pdf");
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("AccountStatement");
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

