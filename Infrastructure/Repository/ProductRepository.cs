using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Product> CreateByProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteByIdAsync(int Id)
        {
            var findProduct = await _dbContext.Products.FirstOrDefaultAsync(op => op.Id == Id);

            if (findProduct != null)
            {
                // Soft delete: mark as inactive
                findProduct.IsActive = false;
                findProduct.ModifiedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<Product> EditByProductAsync(Product product, int Id)
        {
            var findProduct = await _dbContext.Products.FirstOrDefaultAsync(op => op.Id == Id);

            if (findProduct != null)
            {
                // Update only allowed fields
                findProduct.Price = product.Price;
                findProduct.Descriptions = product.Descriptions;
                findProduct.IsActive = product.IsActive;
                findProduct.ModifiedDate = DateTime.Now; // always set on update

                await _dbContext.SaveChangesAsync();

                return findProduct; // return updated entity
            }

            return null; // better than returning input product if not found
        }


        public async Task<List<Product>> GetAsync()
        {
            return await _dbContext.Products.Where(op=>op.IsActive==true).ToListAsync();
        }

        public async Task<List<Product>> GetByUserIdAsync(string UserId)
        {
            return await _dbContext.Products.Where(op => op.UserId == UserId).ToListAsync();
        }
        public async Task<Product> GetProductById(int Id)
        {
            return await _dbContext.Products.Where(op => op.Id == Id).FirstOrDefaultAsync();
        }
    }
}
