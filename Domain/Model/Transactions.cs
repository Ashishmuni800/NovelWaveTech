using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public enum TransactionType
    {
        Credit = 1,
        Debit = 2
    }

    public class Transactions
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; }
    }
}
