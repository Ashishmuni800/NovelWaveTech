using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions
{
    public static class PermissionClaims
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c => c.Type == "Permission" && c.Value == permission);
        }
    }

}
