using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class ProductViewModelData
    {
        public int Id { get; set; }
        public bool EditMinutes { get; set; }
        public decimal Price { get; set; }
        public decimal SumOfPrice { get; set; }
        public decimal SumOfPriceBycreater { get; set; }
        public string Descriptions { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsOwner { get; set; }
        public bool IsActive { get; set; }
        public string CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
