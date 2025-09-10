using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class Product2DTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Descriptions { get; set; }
        //public string UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
