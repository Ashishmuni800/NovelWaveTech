using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface IProductRepository
    {
        Task<bool> DeleteByIdAsync(int Id);
        Task<List<Product>> GetByUserIdAsync(string UserId);
        Task<Product> GetProductById(int Id);
        Task<List<Product>> GetAsync();
        Task<Product> CreateByProductAsync(Product product);
        Task<Product> EditByProductAsync(Product product,int Id);
    }
}
