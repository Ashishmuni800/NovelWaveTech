using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ServiceInfraRepo : IServiceInfraRepo
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly int _JwtExpiry;
        private readonly ApplicationDbContext _dbContext;
        public ServiceInfraRepo(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            AuthRepo = new UserAuthRepository(_userManager, _signInManager, configuration,_dbContext);
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = int.Parse(configuration["Jwt:ExpiryMinutes"]);
        }
        public IUserAuthRepository AuthRepo { get; set; }
    }
}
