using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class PermissionMatrixViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> AllPermissions { get; set; } = new();
        public List<string> AssignedPermissions { get; set; } = new();
        public List<string> SelectedPermissions { get; set; } = new(); // for post-back
    }

}
