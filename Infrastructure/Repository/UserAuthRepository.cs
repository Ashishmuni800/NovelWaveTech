using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly int _JwtExpiry;

        public UserAuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = int.Parse(configuration["Jwt:ExpiryMinutes"]);
        }

        //public async Task<LoginModel> CheckPasswordSignInAsync(ApplicationUser user, string Password)
        //{
        //    var result = await _signInManager.CheckPasswordSignInAsync(user, Password, false);
        //}

        public async Task<RegisterModel> FindByEmailAsync(string Email)
        {
            RegisterModel register=new RegisterModel();
            register.Email=Email;
            var result= await _userManager.FindByEmailAsync(Email);
            if (result == null)
                return null;
            else
            {
                return register;
            }
        }

        public Task<LoginModel> LoginAsync(LoginModel user)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterModel> RegisterAsync(RegisterModel registerModel)
        {
            var user = new ApplicationUser
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                Name = registerModel.Name
            };
            await _userManager.CreateAsync(user, registerModel.Password);
            return registerModel;
        }
    }
}
