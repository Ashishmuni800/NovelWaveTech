using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CustomerBalanceDTO
    {
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Balance => TotalCredit - TotalDebit;
    }

}
