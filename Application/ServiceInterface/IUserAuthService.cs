using Application.DTO;
using Domain.Model;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface IUserAuthService
    {
        Task<RegisterDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<RegisterDTO> FindByEmailAsync(string Email);
        Task<ApplicationUser> FindByEmailUserAsync(string Email);
        Task<LoginDTO> LoginAsync(LoginDTO loginDTO);
        Task<ApplicationUser> CheckPasswordSignInAsync(ApplicationUser user, string Password);
        //Task<string> GeneratedJwtToken(ApplicationUser user);
        void Logout();
    }
}
