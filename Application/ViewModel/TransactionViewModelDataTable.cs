using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class TransactionViewModelDataTable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public string TransactionDate { get; set; }
    }
}
