using Application.DTO;
using Application.ViewModel;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface IProductServices
    {
        Task<bool> DeleteByIdAsync(int Id);
        Task<List<ProductViewModel>> GetByUserIdAsync(string UserId);
        Task<List<ProductSummaryDTO>> GetSumByUserIdAsync();
        Task<decimal> GetSumAsync();
        Task<ProductViewModel> GetProductById(int Id);
        Task<List<ProductViewModel>> GetAsync();
        Task<ProductDTO> CreateByProductAsync(ProductDTO product);
        Task<ProductDTO> EditByProductAsync(ProductDTO product, int Id);
    }
}
