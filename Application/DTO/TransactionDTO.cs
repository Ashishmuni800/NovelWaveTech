using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    // Models/Transaction.cs
    public enum TransactionType
    {
        Credit = 1,
        Debit = 2
    }

    public class TransactionDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public CustomerDTO Customer { get; set; }
    }

}
