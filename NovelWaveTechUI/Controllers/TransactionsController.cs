using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using Domain.Model;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SelectPdf;
using Syncfusion.Pdf.Security;
using System.Net.Http;
using System.Text;

namespace NovelWaveTechUI.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IHttpClients _httpClient;

        public TransactionsController(IConfiguration configuration, IHttpClients httpClient
            ,ILogger<TransactionsController> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
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
                    if (accountNumber=="All")
                    {
                        string baseUrl = _configuration["BaseUrl"];
                        string fullUrls = $"{baseUrl}/api/customers/Transactions/GetBalance";

                        var responses = await _httpClient.GetAsync(fullUrls, true);
                        var product = JsonConvert.DeserializeObject<CustomerBalanceDTO>(responses);
                        //ViewBag.data = product;
                        return View(product);
                    }
                    else
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

        //[HttpGet]
        //public async Task<IActionResult> DownloadPdf(string accountNumber)
        //{
        //    var token = Request.Cookies["AuthToken"];
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    if (string.IsNullOrWhiteSpace(accountNumber))
        //    {
        //        return RedirectToAction("AccountStatement");
        //    }

        //    try
        //    {
        //        string baseUrl = _configuration["BaseUrl"];
        //        if (string.IsNullOrEmpty(baseUrl))
        //        {
        //            _logger.LogError("BaseUrl is not configured.");
        //            return StatusCode(500, "Configuration error");
        //        }

        //        // Get Customer
        //        var customerResponse = await _httpClient.GetAsync($"{baseUrl}/api/customers/{accountNumber}");
        //        if (!customerResponse.Any())
        //        {
        //            _logger.LogWarning("Failed to fetch customer");
        //            return RedirectToAction("AccountStatement");
        //        }
        //        var customer = JsonConvert.DeserializeObject<CustomerViewModel>(customerResponse);

        //        if (string.IsNullOrEmpty(customer?.AccountNumber))
        //        {
        //            return RedirectToAction("AccountStatement");
        //        }

        //        // Get Transactions
        //        var txResponse = await _httpClient.GetAsync($"{baseUrl}/api/customers/Transactions/customerId/{customer.Id}");
        //        if (!txResponse.Any())
        //        {
        //            _logger.LogWarning("Failed to fetch transactions");
        //            return RedirectToAction("AccountStatement");
        //        }
        //        var transactions = JsonConvert.DeserializeObject<List<TransactionViewModelDataTable>>(txResponse);

        //        // Generate PDF with QuestPDF
        //        var pdfDoc = Document.Create(container =>
        //        {
        //            container.Page(page =>
        //            {
        //                page.Margin(20);

        //                page.Header().PaddingBottom(15)
        //                            .AlignCenter()
        //                            .Text("Account Statement")
        //                            .FontSize(18)
        //                            .Underline()
        //                            .Bold()
        //                            .FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

        //                page.Content().Table(table =>
        //                {
        //                    // Define columns
        //                    table.ColumnsDefinition(c =>
        //                    {
        //                        c.RelativeColumn(1);    // Amount
        //                        c.RelativeColumn(1.5f); // Type
        //                        c.RelativeColumn(2);    // Notes
        //                        c.RelativeColumn(1.5f); // Transaction Date
        //                    });

        //                    // Header row
        //                    table.Header(h =>
        //                    {
        //                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Amount").Bold();
        //                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Type").Bold();
        //                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Notes").Bold();
        //                        h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Transaction Date").Bold();
        //                    });

        //                    // Data rows with zebra striping
        //                    for (int i = 0; i < transactions.Count; i++)
        //                    {
        //                        var item = transactions[i];
        //                        var bgColor = (i % 2 == 0) ? QuestPDF.Helpers.Colors.White : QuestPDF.Helpers.Colors.Grey.Lighten4;

        //                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Amount.ToString("C2")); // formatted as currency
        //                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Type ?? "");
        //                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Notes ?? "");
        //                        table.Cell().Background(bgColor).Border(1).Padding(5).Text(
        //                            string.IsNullOrEmpty(item.TransactionDate) ? "" : DateTime.Parse(item.TransactionDate).ToString("dd-MM-yyyy")
        //                        );
        //                    }
        //                });


        //                page.Footer().Row(row =>
        //                {
        //                    row.RelativeColumn().AlignLeft().Text($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}");
        //                    row.RelativeColumn().AlignRight().Text(txt =>
        //                    {
        //                        txt.CurrentPageNumber();
        //                        txt.Span(" / ");
        //                        txt.TotalPages();
        //                    });
        //                });
        //            });
        //        });

        //        var pdfBytes = pdfDoc.GeneratePdf();

        //        // Optional: Encrypt with iText7
        //        try
        //        {
        //            using var input = new MemoryStream(pdfBytes);
        //            using var output = new MemoryStream();

        //            var ownerPassword = "MySecureAdminPassword";

        //            using (var reader = new PdfReader(input))
        //            {
        //                var writerProps = new WriterProperties()
        //                    .SetStandardEncryption(
        //                        Encoding.UTF8.GetBytes(accountNumber),
        //                        Encoding.UTF8.GetBytes(ownerPassword),
        //                        EncryptionConstants.ALLOW_PRINTING,
        //                        EncryptionConstants.ENCRYPTION_AES_256
        //                    );

        //                using var writer = new PdfWriter(output, writerProps);
        //                using var doc = new PdfDocument(reader, writer);
        //                doc.Close();
        //            }

        //            return File(output.ToArray(), "application/pdf", "AccountStatement.pdf");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "PDF encryption failed. Returning unencrypted version.");
        //            return File(pdfBytes, "application/pdf", "AccountStatement.pdf");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unhandled error in DownloadPdf");
        //        return StatusCode(500, "An error occurred while generating the PDF.");
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> DownloadPdf(string accountNumber)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return RedirectToAction("AccountStatement");
            }

            try
            {
                string baseUrl = _configuration["BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("BaseUrl is not configured.");
                    return StatusCode(500, "Configuration error");
                }

                // ✅ Get customer JSON (string)
                var customerJson = await _httpClient.GetAsync($"{baseUrl}/api/customers/{accountNumber}");
                if (customerJson.StartsWith("4") || customerJson.StartsWith("5")) // crude check for status code
                {
                    _logger.LogWarning("Failed to fetch customer: " + customerJson);
                    return RedirectToAction("AccountStatement");
                }

                var customer = JsonConvert.DeserializeObject<CustomerViewModel>(customerJson);
                if (customer == null)
                {
                    _logger.LogWarning("Deserialized customer is null");
                    return RedirectToAction("AccountStatement");
                }

                // ✅ Get transactions JSON (string)
                var txJson = await _httpClient.GetAsync($"{baseUrl}/api/customers/Transactions/customerId/{customer.Id}");
                if (txJson.StartsWith("4") || txJson.StartsWith("5"))
                {
                    _logger.LogWarning("Failed to fetch transactions: " + txJson);
                    return RedirectToAction("AccountStatement");
                }

                var transactions = JsonConvert.DeserializeObject<List<TransactionViewModelDataTable>>(txJson);

                // ✅ Build HTML content
                var html = new StringBuilder();
                html.Append("<h2>Account Statement</h2>");
                html.Append("<table border='1' cellpadding='5' cellspacing='0' style='width:100%; border-collapse: collapse;'>");
                html.Append("<tr><th>Amount</th><th>Type</th><th>Notes</th><th>Transaction Date</th></tr>");

                foreach (var t in transactions)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{t.Amount.ToString("C2")}</td>");
                    html.Append($"<td>{t.Type ?? ""}</td>");
                    html.Append($"<td>{t.Notes ?? ""}</td>");
                    html.Append($"<td>{(string.IsNullOrEmpty(t.TransactionDate) ? "" : DateTime.Parse(t.TransactionDate).ToString("dd-MM-yyyy"))}</td>");
                    html.Append("</tr>");
                }

                html.Append("</table>");
                html.Append($"<p>Generated: {DateTime.Now:dd-MM-yyyy HH:mm}</p>");

                // ✅ Generate PDF
                var converter = new HtmlToPdf();
                var doc = converter.ConvertHtmlString(html.ToString());

                // ✅ Add PDF Security
                doc.Security.UserPassword = accountNumber;
                doc.Security.OwnerPassword = "AdminSecurePassword";
                doc.Security.CanPrint = true;
                doc.Security.CanAccessibilityCopyContent = false;
                doc.Security.CanAssembleDocument = false;
                doc.Security.CanCopyContent = false;
                doc.Security.CanEditContent = false;
                doc.Security.CanEditAnnotations = false;

                // ✅ Return File
                var pdfBytes = doc.Save();
                doc.Close();

                return File(pdfBytes, "application/pdf", "AccountStatement.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in DownloadPdf");
                return StatusCode(500, "An error occurred while generating the PDF.");
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

