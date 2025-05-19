using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Context
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
