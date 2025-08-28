using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions
{
    public static class Permissions
    {
        public static List<string> All = new()
    {
        "ViewUsers",
        "EditUsers",
        "DeleteUsers",
        "ManageRoles",
        "AccessReports",
        "SystemSettings"
    };
    }
}
