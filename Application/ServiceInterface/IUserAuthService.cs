using Application.DTO;
using Application.ViewModel;
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
        Task<RegisterDTO> FindByUserNameAsync(string Name);
        Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        //Task<LoginDTO> LoginAsync(LoginDTO loginDTO);
        Task<IEnumerable<PasswordChangeHistoryDTO>> GetByPasswordChangeHistoryAsync(string UserPassword);
        Task<PasswordChangeHistoryDTO> CreateByPasswordChangeHistoryAsync(PasswordChangeHistoryDTO passwordChangeHistoryDTO);
        Task<GenerateCaptchaCodeDTO> CreateByGenerateCaptchaCodeAsync(GenerateCaptchaCodeDTO captchaCodeDTO);
        Task<GenerateCaptchaCodeViewModel> GetByGenerateCaptchaCodeAsync(string captchaCode);
        Task<ApplicationUser> CheckPasswordSignInAsync(ApplicationUser user, string Password);
        Task<bool> DeleteByGenerateCaptchaCodeAsync();
        //Task<string> GeneratedJwtToken(ApplicationUser user);
        //void Logout();
    }
}
