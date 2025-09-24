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
    public interface ICustomerService
    {
        Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDTO);
        Task<CustomerViewModel> GetCustomerByIdAsync(Guid Id);
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync();
        Task<CustomerDTO> UpdateCustomerAsync(CustomerDTO customerDTO, Guid Id);
        Task<bool> DeleteCustomerAsync(Guid Id);
    }

}
