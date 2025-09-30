using Application.DTO;
using Application.Service;
using Application.ServiceInterface;
using Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NovelWaveTechAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IServiceInfra _CustomerService;
        public CustomersController(IServiceInfra CustomerService)
        {
            _CustomerService = CustomerService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _CustomerService.CustomerService.GetAllCustomersAsync();
            var setdata = new List<CustomerDataViewModel>();
            foreach (var item in customers)
            {
                setdata.Add(new CustomerDataViewModel
                {
                    Name = item.Name,
                    AccountNumber = item.AccountNumber,
                    PhoneNumber = item.PhoneNumber,
                    CreatedAt = item.CreatedAt.ToString("dd-MM-yyyy"),
                });
            }
            return Ok(setdata);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var customer = await _CustomerService.CustomerService.GetCustomerByIdAsync(id);
        //    if (customer == null)
        //        return NotFound();

        //    return Ok(customer);
        //}
        [Authorize]
        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(string accountNumber)
        {
            var customer = await _CustomerService.CustomerService.GetCustomerByAccountNumberAsync(accountNumber);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDTO dto)
        {
            var AccountNumber = await _CustomerService.AccountNumberGenerator.GenerateAccountNumberAsync();
            var Customer = new CustomerDTO
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                AccountNumber = AccountNumber
            };
            var created = await _CustomerService.CustomerService.CreateCustomerAsync(Customer);
            return Ok(created);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerDTO dto)
        {
            var updated = await _CustomerService.CustomerService.UpdateCustomerAsync(dto, id);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _CustomerService.CustomerService.DeleteCustomerAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
