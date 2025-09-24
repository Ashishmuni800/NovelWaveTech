using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    // Models/Reminder.cs
    public class ReminderDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public string Message { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool Sent { get; set; } = false;

        public CustomerDTO Customer { get; set; }
    }

}
