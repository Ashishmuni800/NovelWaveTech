using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public string Message { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool Sent { get; set; } = false;

        public Customer Customer { get; set; }
    }
}
