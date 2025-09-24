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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            // Ensure AccountNumber is valid
            if (customer.AccountNumber.Length != 11)
                throw new ArgumentException("Account number must be exactly 11 characters.");

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null)
                return false;

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.Transactions)
                .Include(c => c.Reminders)
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid id)
        {
            return await _dbContext.Customers.Where(op => op.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Customer> UpdateCustomerAsync(Customer updatedCustomer, Guid id)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(id);
            if (existingCustomer == null)
                return null;

            // Optional: Validate account number again
            if (updatedCustomer.AccountNumber.Length != 11)
                throw new ArgumentException("Account number must be exactly 11 characters.");

            // Update fields
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.PhoneNumber = updatedCustomer.PhoneNumber;
            existingCustomer.AccountNumber = updatedCustomer.AccountNumber;
            // Do not update CreatedAt or Id
             //_dbContext.Customers.Update(existingCustomer);
            await _dbContext.SaveChangesAsync();
            return existingCustomer;
        }
    }

}
