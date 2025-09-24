using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> GetCustomerByIdAsync(Guid Id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> UpdateCustomerAsync(Customer customer, Guid Id);
        Task<bool> DeleteCustomerAsync(Guid Id);
    }
}
