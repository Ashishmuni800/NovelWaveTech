using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IServiceInfraRepo _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(IServiceInfraRepo customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDTO)
        {
            if (customerDTO.AccountNumber.Length != 11)
                throw new ArgumentException("Account number must be exactly 11 characters.");

            var customerEntity = _mapper.Map<Customer>(customerDTO);

            var createdCustomer = await _customerRepository.CustomerRepo.CreateCustomerAsync(customerEntity);
            return _mapper.Map<CustomerDTO>(createdCustomer);
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            return await _customerRepository.CustomerRepo.DeleteCustomerAsync(id);
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.CustomerRepo.GetAllCustomersAsync();
            return _mapper.Map<IEnumerable<CustomerViewModel>>(customers);
        }

        public async Task<CustomerViewModel> GetCustomerByAccountNumberAsync(string accountNumber)
        {
            var customer = await _customerRepository.CustomerRepo.GetCustomerByAccountNumberAsync(accountNumber);
            if (customer == null)
                return null;

            return _mapper.Map<CustomerViewModel>(customer);
        }

        public async Task<CustomerViewModel> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerRepository.CustomerRepo.GetCustomerByIdAsync(id);
            if (customer == null)
                return null;

            return _mapper.Map<CustomerViewModel>(customer);
        }

        public async Task<CustomerDTO> UpdateCustomerAsync(CustomerDTO customerDTO, Guid id)
        {
            if (customerDTO.AccountNumber.Length != 11)
                throw new ArgumentException("Account number must be exactly 11 characters.");

            var updatedEntity = _mapper.Map<Customer>(customerDTO);

            var updatedCustomer = await _customerRepository.CustomerRepo.UpdateCustomerAsync(updatedEntity, id);
            if (updatedCustomer == null)
                return null;

            return _mapper.Map<CustomerDTO>(updatedCustomer);
        }
    }

}
