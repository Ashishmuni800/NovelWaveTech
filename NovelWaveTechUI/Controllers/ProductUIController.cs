using Application.ApiHttpClient;
using Application.DTO;
using Application.ViewModel;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using System.Security.Claims;
using System.Text;

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
                return RedirectToAction("Login","Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/GetProducts";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var product = JsonConvert.DeserializeObject<List<ProductViewModelData>>(response);

                string fullUrls = $"{baseUrl}/api/Products/GetProductsSumByUserId";
                var response2 = await _httpClient.GetAsync(fullUrls, true);
                var product2 = JsonConvert.DeserializeObject<List<ProductSummaryDTO2>>(response2);
                ViewBag.data = product2;
                return View(product);
            }
        }
        public async Task<IActionResult> GetByUserId()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
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
        public async Task<IActionResult> Create([FromBody] Product2DTO productDTO)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
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
        //[HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string baseUrl = _configuration["BaseUrl"];
                string fullUrl = $"{baseUrl}/api/Products/GetProducts";
                var response = await _httpClient.GetAsync(fullUrl, true);
                var products = JsonConvert.DeserializeObject<List<ProductViewModelData>>(response);

               // var products = await _ProductService.ProductService.GetAsync();
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Products");

                // Header
                //worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 1).Value = "Price";
                worksheet.Cell(1, 2).Value = "Description";
                worksheet.Cell(1, 3).Value = "Creater Name";
                worksheet.Cell(1, 4).Value = "IsActive";
                worksheet.Cell(1, 5).Value = "Created Date";

                int row = 2;
                foreach (var item in products)
                {
                    //worksheet.Cell(row, 1).Value = item.Id;
                    worksheet.Cell(row, 1).Value = item.Price;
                    worksheet.Cell(row, 2).Value = item.Descriptions;
                    worksheet.Cell(row, 3).Value = item.Name;
                    worksheet.Cell(row, 4).Value = item.IsActive ? "Yes" : "No";
                    worksheet.Cell(row, 5).Value = item.CreatedDate;//.ToString("dd-MM-yyyy");
                    row++;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Products.xlsx");
            }
            
        }


        public async Task<IActionResult> ExportPdf()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/Products/GetProducts";
            var response = await _httpClient.GetAsync(fullUrl, true);
            var products = JsonConvert.DeserializeObject<List<ProductViewModelData>>(response);

            var pdfDoc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Header
                    page.Header().PaddingBottom(15)
                        .AlignCenter()
                        .Text("Product List")
                        .FontSize(18)
                        .Bold()
                        .FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                    // Content
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(1);    // Price
                            c.RelativeColumn(2);    // Description
                            c.RelativeColumn(2);    // Creator Name
                            c.RelativeColumn(1);    // IsActive
                            c.RelativeColumn(1.5f); // Created Date
                        });

                        // Header row
                        table.Header(h =>
                        {
                            h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Price").Bold();
                            h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Description").Bold();
                            h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Creator Name").Bold();
                            h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("IsActive").Bold();
                            h.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Border(1).Padding(5).Text("Created Date").Bold();
                        });

                        // Data rows with zebra striping
                        for (int i = 0; i < products.Count; i++)
                        {
                            var item = products[i];
                            var bgColor = (i % 2 == 0) ? QuestPDF.Helpers.Colors.White : QuestPDF.Helpers.Colors.Grey.Lighten4;

                            table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Price.ToString());
                            table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Descriptions ?? "");
                            table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.Name ?? "");
                            table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.IsActive ? "Yes" : "No");
                            table.Cell().Background(bgColor).Border(1).Padding(5).Text(item.CreatedDate ?? "");
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

            //var pdf = doc.GeneratePdf();
            //return File(pdf, "application/pdf", "Products.pdf");

            var pdfBytes = pdfDoc.GeneratePdf();

            using var input = new MemoryStream(pdfBytes);
            using var output = new MemoryStream();

            var ownerPassword = "MySecureAdminPassword";

            using (var reader = new PdfReader(input))
            {
                var writerProps = new WriterProperties()
                    .SetStandardEncryption(
                        Encoding.UTF8.GetBytes(products[0].Name),
                        Encoding.UTF8.GetBytes(ownerPassword),
                        EncryptionConstants.ALLOW_PRINTING,
                        EncryptionConstants.ENCRYPTION_AES_256
                    );

                using var writer = new PdfWriter(output, writerProps);
                using var doc = new PdfDocument(reader, writer);
                doc.Close();
            }

            return File(output.ToArray(), "application/pdf", "Products.pdf");
        }

        public async Task<IActionResult> ExportCsv()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            string baseUrl = _configuration["BaseUrl"];
            string fullUrl = $"{baseUrl}/api/Products/GetProducts";
            var response = await _httpClient.GetAsync(fullUrl, true);
            var products = JsonConvert.DeserializeObject<List<ProductViewModelData>>(response);

            var csvBuilder = new StringBuilder();

            // Header row
            csvBuilder.AppendLine("Price,Description,Creator Name,IsActive,Created Date");

            // Data rows
            foreach (var item in products)
            {
                csvBuilder.AppendLine(
                    $"{item.Price}," +
                    $"\"{item.Descriptions?.Replace("\"", "\"\"")}\"," +
                    $"\"{item.Name?.Replace("\"", "\"\"")}\"," +
                    $"{(item.IsActive ? "Yes" : "No")}," +
                    $"{item.CreatedDate}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv", "Products.csv");
        }


    }
}
