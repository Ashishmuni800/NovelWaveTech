using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface IUserAuthRepository
    {
        Task<RegisterModel> RegisterAsync(RegisterModel registerModel);
        Task<LoginModel> LoginAsync(LoginModel user);
        Task<RegisterModel> FindByEmailAsync(string Email);
        //Task<LoginModel> CheckPasswordSignInAsync(ApplicationUser user, string Password);
        void Logout();
    }
}
