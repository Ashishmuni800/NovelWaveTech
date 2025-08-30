using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UserPermission
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Permission { get; set; }
    }
}
