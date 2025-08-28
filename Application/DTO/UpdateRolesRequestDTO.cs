using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateRolesRequestDTO
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
