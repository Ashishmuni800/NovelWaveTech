using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CustomerBalance
    {
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Balance => TotalCredit - TotalDebit;
    }

}
