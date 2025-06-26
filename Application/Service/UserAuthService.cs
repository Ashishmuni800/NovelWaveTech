using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IServiceInfraRepo _userAuthRepository;
        private readonly IMapper _Mapp;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserAuthService(IServiceInfraRepo userAuthRepository, IMapper Mapp, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager )
        {
            _userAuthRepository = userAuthRepository;
            _Mapp = Mapp;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            // find user with this username
            var user = await _userManager.FindByNameAsync(changePasswordDTO.UserName).ConfigureAwait(false);
            //if (user?.Approved != true || user.IsActive == false) return false;
            //change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDTO.Password, changePasswordDTO.NewPassword).ConfigureAwait(false);
            //set password status in user
            // user.ChangePassword = false;
            //user.EncSecret = model.EncSecret;
            //var userPasswordStatusResult = await _userManager.UpdateAsync(user);
            //return true only if both are success
            return changePasswordResult.Succeeded;//&& userPasswordStatusResult.Succeeded;
        }


        public async Task<ApplicationUser> CheckPasswordSignInAsync(ApplicationUser user, string Password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, Password, false);
            if (!result.Succeeded)
            {
                return null;
            }
            return user;
        }

        public async Task<GenerateCaptchaCodeDTO> CreateByGenerateCaptchaCodeAsync(GenerateCaptchaCodeDTO captchaCodeDTO)
        {
            if(captchaCodeDTO == null)
                throw new ArgumentNullException(nameof(captchaCodeDTO));

            if (_Mapp == null)
                throw new InvalidOperationException("_Mapp is not initialized.");

            if (_userAuthRepository?.AuthRepo == null)
                throw new InvalidOperationException("_userAuthRepository.AuthRepo is not initialized.");

            var model = _Mapp.Map<GenerateCaptchaCode>(captchaCodeDTO);

            var result = await _userAuthRepository.AuthRepo
                .CreateByGenerateCaptchaCodeAsync(model)
                .ConfigureAwait(false);

            var dto = _Mapp.Map<GenerateCaptchaCodeDTO>(result);
            return dto;
        }

        // In your service class
        public async Task<PasswordChangeHistoryDTO> CreateByPasswordChangeHistoryAsync(PasswordChangeHistoryDTO passwordChangeHistoryDTO)
        {
            if (passwordChangeHistoryDTO == null)
                throw new ArgumentNullException(nameof(passwordChangeHistoryDTO));

            if (_Mapp == null)
                throw new InvalidOperationException("_Mapp is not initialized.");

            if (_userAuthRepository?.AuthRepo == null)
                throw new InvalidOperationException("_userAuthRepository.AuthRepo is not initialized.");

            var model = _Mapp.Map<PasswordChangeHistory>(passwordChangeHistoryDTO);

            var result = await _userAuthRepository.AuthRepo
                .CreateByPasswordChangeHistoryAsync(model)
                .ConfigureAwait(false);

            var dto = _Mapp.Map<PasswordChangeHistoryDTO>(result);
            return dto;
        }

        public async Task<bool> DeleteByGenerateCaptchaCodeAsync()
        {
            var result = await _userAuthRepository.AuthRepo.DeleteByGenerateCaptchaCodeAsync().ConfigureAwait(false);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<RegisterDTO> FindByEmailAsync(string Email)
        {
            var model = _Mapp.Map<RegisterModel>(Email);
            var result=await _userAuthRepository.AuthRepo.FindByEmailAsync(Email).ConfigureAwait(false);
            if (result != null) 
            {
               var dto =_Mapp.Map<RegisterDTO>(model);
                return dto;
            }
            return null;
        }

        public async Task<ApplicationUser> FindByEmailUserAsync(string Email)
        {
            var result = await _userManager.FindByEmailAsync(Email);
            return result;
        }

        public async Task<RegisterDTO> FindByUserNameAsync(string Name)
        {
            var model = _Mapp.Map<RegisterModel>(Name);
            var result = await _userAuthRepository.AuthRepo.FindByUserNameAsync(Name).ConfigureAwait(false);
            if (result != null)
            {
                var dto = _Mapp.Map<RegisterDTO>(model);
                return dto;
            }
            return null;
        }

        public async Task<GenerateCaptchaCodeViewModel> GetByGenerateCaptchaCodeAsync(string captchaCode)
        {
            var result = await _userAuthRepository.AuthRepo.GetByGenerateCaptchaCodeAsync(captchaCode).ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<GenerateCaptchaCodeViewModel>(result);
                return dtoList;
            }

            return null;
        }

        public async Task<IEnumerable<PasswordChangeHistoryDTO>> GetByPasswordChangeHistoryAsync(string userPassword)
        {
            var result = await _userAuthRepository.AuthRepo.GetByPasswordChangeHistoryAsync(userPassword).ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<IEnumerable<PasswordChangeHistoryDTO>>(result);
                return dtoList;
            }

            return Enumerable.Empty<PasswordChangeHistoryDTO>();
        }


        public async Task<RegisterDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var model = _Mapp.Map<RegisterModel>(registerDTO);
            await _userAuthRepository.AuthRepo.RegisterAsync(model).ConfigureAwait(false);
            var dto = _Mapp.Map<RegisterDTO>(model);
            return dto;
        }
    }
}
