using Application.DTO;
using Application.ServiceInterface;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _CustomerService.CustomerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _CustomerService.CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

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
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerDTO dto)
        {
            var updated = await _CustomerService.CustomerService.UpdateCustomerAsync(dto, id);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
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
