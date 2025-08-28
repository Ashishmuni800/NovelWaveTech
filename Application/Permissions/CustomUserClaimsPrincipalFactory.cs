using Application.ServiceInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Permissions
{

    public class CustomUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly ApplicationDbContext _db;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options,
            ApplicationDbContext db) // ✅ use db directly, not service
            : base(userManager, roleManager, options)
        {
            _db = db;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var permissions = await _db.UserPermissions
                .Where(p => p.UserId == user.Id)
                .Select(p => p.Permission)
                .ToListAsync();

            foreach (var permission in permissions)
            {
                identity.AddClaim(new Claim("Permission", permission));
            }

            return identity;
        }
    }

}
