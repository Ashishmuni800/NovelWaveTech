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
        //Task<LoginModel> LoginAsync(LoginModel user);
        Task<RegisterModel> FindByEmailAsync(string Email);
        Task<RegisterModel> FindByUserNameAsync(string Name);
        Task<IEnumerable<PasswordChangeHistory>> GetByPasswordChangeHistoryAsync(string UserPassword);
        Task<GenerateCaptchaCode> GetByGenerateCaptchaCodeAsync(string captchaCode);
        Task<PasswordChangeHistory> CreateByPasswordChangeHistoryAsync(PasswordChangeHistory passwordChangeHistoryModel);
        Task<GenerateCaptchaCode> CreateByGenerateCaptchaCodeAsync(GenerateCaptchaCode captchaCode);
        Task<bool> DeleteByGenerateCaptchaCodeAsync();
        //Task<LoginModel> CheckPasswordSignInAsync(ApplicationUser user, string Password);
        //void Logout();
    }
}
