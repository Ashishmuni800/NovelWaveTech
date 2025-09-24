using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }    // Should be 11 characters
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Transactions> Transactions { get; set; }
        public ICollection<Reminder> Reminders { get; set; }
    }
}
