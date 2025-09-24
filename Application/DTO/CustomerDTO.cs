using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    // Models/Customer.cs
    public class CustomerDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }    // Should be 11 characters
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TransactionDTO> Transactions { get; set; }
        public ICollection<ReminderDTO> Reminders { get; set; }
    }


}
