using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> AssignedRoles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
    }

}
