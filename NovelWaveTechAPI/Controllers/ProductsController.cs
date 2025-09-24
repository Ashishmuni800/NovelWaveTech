using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using Domain.Model;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System.Security.Claims;

namespace NovelWaveTechAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceInfra _ProductService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController(IServiceInfra ProductService, UserManager<ApplicationUser> userManager)
        {
            _ProductService = ProductService;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductsSumByUserId()
        {
            var products3 = await _ProductService.ProductService.GetSumByUserIdAsync().ConfigureAwait(false);
            var setdata = new List<ProductSummaryDTO2>();
            foreach (var item in products3)
            {
                var productUser = await _userManager.FindByIdAsync(item.UserId);
                setdata.Add(new ProductSummaryDTO2

                {
                    UserId = item.UserId,
                    Name = productUser?.Name,
                    Products = item.Products,
                    TotalPrice=item.TotalPrice
                });
            }
            return Ok(setdata);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _ProductService.ProductService.GetAsync().ConfigureAwait(false);
            var products2 = await _ProductService.ProductService.GetSumAsync().ConfigureAwait(false);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var setdata = new List<ProductViewModelData>();

            foreach (var item in products)
            {
                var productUser = await _userManager.FindByIdAsync(item.UserId);

                if (roles.Contains("Admin"))
                {
                    setdata.Add(new ProductViewModelData
                    {
                        Id = item.Id,
                        Price = item.Price,
                        Descriptions = item.Descriptions,
                        CreatedDate = item.CreatedDate.ToString("dd-MM-yyyy"),
                        Name = productUser?.Name,
                        IsActive = item.IsActive,
                        EditMinutes = true,
                        UserId = item.UserId,
                        IsOwner = true,
                        SumOfPrice= products2
                    });
                }
                else
                {
                    setdata.Add(new ProductViewModelData
                    {
                        Id = item.Id,
                        Price = item.Price,
                        Descriptions = item.Descriptions,
                        CreatedDate = item.CreatedDate.ToString("dd-MM-yyyy"),
                        Name = productUser?.Name,
                        IsActive = item.IsActive,
                        EditMinutes = (DateTime.Now - item.CreatedDate) < TimeSpan.FromMinutes(5),
                        UserId = item.UserId,
                        IsOwner = (item.UserId == userId),
                        SumOfPrice = products2
                    });
                }
            }
            return Ok(setdata);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductByUserId()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = await _ProductService.ProductService.GetByUserIdAsync(UserId).ConfigureAwait(false);
            return Ok(product);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductById(int Id)
        {
            var product = await _ProductService.ProductService.GetProductById(Id).ConfigureAwait(false);
            return Ok(product);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteProductById(int Id)
        {
            if(Id == 0) return BadRequest("Invalid input");
            var product = await _ProductService.ProductService.DeleteByIdAsync(Id).ConfigureAwait(false);
            if (product == null) return BadRequest("Delete failed");
            return Ok(product);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product2DTO productDTO)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = new ProductDTO()
            {
                Price = productDTO.Price,
                Descriptions = productDTO.Descriptions,
                UserId=UserId,
                IsActive=true,
                CreatedDate= DateTime.Now
            };
            var products = await _ProductService.ProductService.CreateByProductAsync(product).ConfigureAwait(false);
            if (products == null) return BadRequest("Create failed");
            return Ok(products);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProduct([FromBody] Product2DTO productDTO)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = new ProductDTO()
            {
                Id = productDTO.Id,
                Price = productDTO.Price,
                Descriptions = productDTO.Descriptions,
                IsActive = productDTO.IsActive,
                CreatedDate = productDTO.CreatedDate,
                ModifiedDate = DateTime.Now,
                UserId= UserId
            };
            var products = await _ProductService.ProductService.EditByProductAsync(product, product.Id).ConfigureAwait(false);
            if (products == null) return BadRequest("Edit failed");
            return Ok(products);
        }
    }
}
