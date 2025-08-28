using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> Map = new()
    {
        { "Admin", new List<string> { "ViewUsers", "EditUsers", "DeleteUsers", "ManageRoles", "AccessReports", "SystemSettings" } },
        { "Manager", new List<string> { "ViewUsers", "EditUsers", "AccessReports" } },
        { "Support", new List<string> { "ViewUsers" } }
    };

        public static List<string> GetPermissionsForRoles(IEnumerable<string> roles)
        {
            return roles
                .Where(Map.ContainsKey)
                .SelectMany(r => Map[r])
                .Distinct()
                .ToList();
        }
    }

}
