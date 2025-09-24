using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class RemindersRepository : IRemindersRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RemindersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            await _dbContext.Reminders.AddAsync(reminder);
            await _dbContext.SaveChangesAsync();
            return reminder;
        }

        public async Task<bool> DeleteReminderAsync(Guid customerId, Guid id)
        {
            var reminder = await _dbContext.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.CustomerId == customerId);

            if (reminder == null)
                return false;

            _dbContext.Reminders.Remove(reminder);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _dbContext.Reminders
                .Include(r => r.Customer)
                .ToListAsync();
        }

        public async Task<Reminder> GetReminderByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.Reminders
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);
        }

        public async Task<Reminder> GetReminderByIdAsync(Guid id)
        {
            return await _dbContext.Reminders
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reminder> UpdateReminderAsync(Reminder updatedReminder, Guid customerId, Guid id)
        {
            var existing = await _dbContext.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.CustomerId == customerId);

            if (existing == null)
                return null;

            existing.Message = updatedReminder.Message;
            existing.ReminderDate = updatedReminder.ReminderDate;
            existing.Sent = updatedReminder.Sent;

            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }

}
