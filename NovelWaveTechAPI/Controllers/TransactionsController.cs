using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NovelWaveTechAPI.Controllers
{
    [ApiController]
    //[Route("api/customers/{customerId}/[controller]")]
    [Route("api/customers/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IServiceInfra _TransactionsService;

        public TransactionsController(IServiceInfra TransactionsService)
        {
            _TransactionsService = TransactionsService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _TransactionsService.TransactionsService.GetAllTransactionAsync();
            var setdata = new List<TransactionViewModelDataTable>();
            foreach (var item in transactions)
            {
                if(Convert.ToInt32(item.Type) == 1)
                {
                    setdata.Add(new TransactionViewModelDataTable
                    {
                        Id = item.Id,
                        CustomerId = item.CustomerId,
                        Amount = item.Amount,
                        Type = "Credit",
                        Notes = item.Notes,
                        TransactionDate = item.TransactionDate.ToString("dd-MM-yyyy"),
                    });
                }
                if (Convert.ToInt32(item.Type) == 2)
                {
                    setdata.Add(new TransactionViewModelDataTable
                    {
                        Id = item.Id,
                        CustomerId = item.CustomerId,
                        Amount = item.Amount,
                        Type = "Debit",
                        Notes = item.Notes,
                        TransactionDate = item.TransactionDate.ToString("dd-MM-yyyy"),
                    });
                }

            }
            return Ok(setdata);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _TransactionsService.TransactionsService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }
        [Authorize]
        [HttpGet("customerId/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var transactions = await _TransactionsService.TransactionsService.GetTransactionBycustomerIdAsync(customerId);
            if (transactions == null)
                return NotFound();
            var setdata = new List<TransactionViewModelDataTable>();
            foreach (var item in transactions)
            {
                if (Convert.ToInt32(item.Type) == 1)
                {
                    setdata.Add(new TransactionViewModelDataTable
                    {
                        Id = item.Id,
                        CustomerId = item.CustomerId,
                        Amount = item.Amount,
                        Type = "Credit",
                        Notes = item.Notes,
                        TransactionDate = item.TransactionDate.ToString("dd-MM-yyyy"),
                    });
                }
                if (Convert.ToInt32(item.Type) == 2)
                {
                    setdata.Add(new TransactionViewModelDataTable
                    {
                        Id = item.Id,
                        CustomerId = item.CustomerId,
                        Amount = item.Amount,
                        Type = "Debit",
                        Notes = item.Notes,
                        TransactionDate = item.TransactionDate.ToString("dd-MM-yyyy"),
                    });
                }

            }
            return Ok(setdata);
        }
        [Authorize]
        [HttpGet("GetBalanceBycustomerId/{customerId}")]
        public async Task<IActionResult> GetBalanceBycustomerId(Guid customerId)
        {
            var transaction = await _TransactionsService.TransactionsService.GetBalanceBycustomerIdAsync(customerId);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }
        [Authorize]
        [HttpPost("{AccountNumber}")]
        public async Task<IActionResult> Create(string AccountNumber, [FromBody] TransactionRequestDTO dto)
        {
            var RequestDTO = new TransactionDTO
            {
                CustomerId = dto.CustomerId,
                Amount = dto.Amount,
                Type = (Application.DTO.TransactionType)dto.Type,
                Notes = dto.Notes
            };
            var created = await _TransactionsService.TransactionsService.CreateTransactionAsync(RequestDTO);
            return Ok(created);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid customerId, Guid id, [FromBody] TransactionDTO dto)
        {
            var updated = await _TransactionsService.TransactionsService.UpdateTransactionAsync(dto, customerId, id);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        [Authorize]
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
