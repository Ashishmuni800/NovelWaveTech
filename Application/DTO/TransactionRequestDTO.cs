using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public enum TransactionTypes
    {
        Credit = 1,
        Debit = 2
    }
    public class TransactionRequestDTO
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public TransactionTypes Type { get; set; }
        public string Notes { get; set; }
        public string accountNumber { get; set; }
    }
}
