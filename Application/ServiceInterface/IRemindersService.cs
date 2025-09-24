using Application.DTO;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface IRemindersService
    {
        Task<ReminderDTO> CreateReminderAsync(ReminderDTO dto);
        Task<bool> DeleteReminderAsync(Guid customerId, Guid id);
        Task<IEnumerable<ReminderDTO>> GetAllRemindersAsync();
        Task<ReminderDTO> GetReminderByCustomerIdAsync(Guid customerId);
        Task<ReminderDTO> GetReminderByIdAsync(Guid id);
        Task<ReminderDTO> UpdateReminderAsync(ReminderDTO dto, Guid customerId, Guid id);
    }
}
