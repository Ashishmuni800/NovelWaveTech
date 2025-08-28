using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UpdatePermissionsRequest
    {
        public string UserId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
