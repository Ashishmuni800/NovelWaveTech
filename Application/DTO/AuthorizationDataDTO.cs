using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AuthorizationDataDTO
    {
        public string Id { get; set; }
        public string token { get; set; }
        public DateTime CreatedDatetime { get; set; }
    }
}
