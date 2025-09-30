using Application.DTO;
using Application.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NovelWaveTechAPI.Controllers
{
    [ApiController]
    //[Route("api/customers/{customerId}/[controller]")]
    [Route("api/customers/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly IServiceInfra _RemindersService;

        public RemindersController(IServiceInfra RemindersService)
        {
            _RemindersService = RemindersService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reminders = await _RemindersService.RemindersService.GetAllRemindersAsync();
            return Ok(reminders);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reminder = await _RemindersService.RemindersService.GetReminderByIdAsync(id);
            if (reminder == null)
                return NotFound();

            return Ok(reminder);
        }
        [Authorize]
        [HttpGet("CustomerId")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var reminder = await _RemindersService.RemindersService.GetReminderByCustomerIdAsync(customerId);
            if (reminder == null)
                return NotFound();

            return Ok(reminder);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Guid customerId, [FromBody] ReminderDTO dto)
        {
            dto.CustomerId = customerId;
            var created = await _RemindersService.RemindersService.CreateReminderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, customerId }, created);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid customerId, Guid id, [FromBody] ReminderDTO dto)
        {
            var updated = await _RemindersService.RemindersService.UpdateReminderAsync(dto, customerId, id);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid customerId, Guid id)
        {
            var success = await _RemindersService.RemindersService.DeleteReminderAsync(customerId, id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }

}
