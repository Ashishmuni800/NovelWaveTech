using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ManageRolesDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; } // Optional in POST, but useful
        public List<string> AssignedRoles { get; set; } = new(); // roles already assigned (GET only)
        public List<string> AllRoles { get; set; } = new();      // all available roles (GET only)

        // ✅ For binding selected roles from checkboxes
        public List<string> SelectedRoles { get; set; } = new();
    }

}
