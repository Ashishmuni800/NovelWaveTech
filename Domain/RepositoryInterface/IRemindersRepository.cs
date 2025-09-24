using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface IRemindersRepository
    {
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(Guid customerId, Guid id);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();
        Task<Reminder> GetReminderByCustomerIdAsync(Guid customerId);
        Task<Reminder> GetReminderByIdAsync(Guid id);
        Task<Reminder> UpdateReminderAsync(Reminder reminder, Guid customerId, Guid id);
    }
}
