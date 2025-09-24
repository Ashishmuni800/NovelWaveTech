using Application.DTO;
using Application.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NovelWaveTechAPI.Controllers
{
    [ApiController]
    [Route("api/customers/{customerId}/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IServiceInfra _TransactionsService;

        public TransactionsController(IServiceInfra TransactionsService)
        {
            _TransactionsService = TransactionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _TransactionsService.TransactionsService.GetAllTransactionAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _TransactionsService.TransactionsService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpGet("first")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var transaction = await _TransactionsService.TransactionsService.GetTransactionBycustomerIdAsync(customerId);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid customerId, [FromBody] TransactionDTO dto)
        {
            dto.CustomerId = customerId;
            var created = await _TransactionsService.TransactionsService.CreateTransactionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, customerId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid customerId, Guid id, [FromBody] TransactionDTO dto)
        {
            var updated = await _TransactionsService.TransactionsService.UpdateTransactionAsync(dto, customerId, id);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid customerId, Guid id)
        {
            var success = await _TransactionsService.TransactionsService.DeleteTransactionAsync(customerId, id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }

}
