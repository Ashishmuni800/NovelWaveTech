using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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

        public async Task<AuthorizationDataDTO> CreateByAuthorizationDataAsync(AuthorizationDataDTO authorizationDataDTO)
        {
            if (authorizationDataDTO == null)
                throw new ArgumentNullException(nameof(authorizationDataDTO));

            if (_Mapp == null)
                throw new InvalidOperationException("_Mapp is not initialized.");

            if (_userAuthRepository?.AuthRepo == null)
                throw new InvalidOperationException("_userAuthRepository.AuthRepo is not initialized.");

            var model = _Mapp.Map<AuthorizationData>(authorizationDataDTO);

            var result = await _userAuthRepository.AuthRepo
                .CreateByAuthorizationDataAsync(model)
                .ConfigureAwait(false);

            var dto = _Mapp.Map<AuthorizationDataDTO>(result);
            return dto;
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

        public async Task<bool> DeleteByAuthorizationDataAsync()
        {
            var result = await _userAuthRepository.AuthRepo.DeleteByAuthorizationDataAsync().ConfigureAwait(false);
            if (result != null)
            {
                return true;
            }
            return false;
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

        public async Task<AuthorizationDataViewModel> GetByAuthorizationDataUserIdAsync(string token)
        {
            var result = await _userAuthRepository.AuthRepo.GetByAuthorizationDataUserIdAsync(token).ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<AuthorizationDataViewModel>(result);
                return dtoList;
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
        public async Task<PaginatedResult<UserDto>> GetUsersIncrementalAsync(int skip, int take)
        {
            var query = _userManager.Users.AsNoTracking().OrderBy(u => u.Id); 

            var list = await query
                .Skip(skip)
                .Take(take + 1)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();

            var hasMore = list.Count > take;
            if (hasMore) list.RemoveAt(take);

            return new PaginatedResult<UserDto>
            {
                items = list,
                hasMore = hasMore
            };
        }

        public async Task<bool> Delete(string Id)
        {
            var data= await _userManager.FindByIdAsync(Id);
            if (data == null) return false;
            var deletedat= await _userManager.DeleteAsync(data);
            if (deletedat == null) return false;
            return true;
        }

        public async Task<UserEditDTO> Edit(UserEditDTO user)
        {
            var data = await _userManager.FindByIdAsync(user.Id);
            if (data == null) return null;
            data.UserName = user.userName;
            data.Email = user.email;
            var updatedat = await _userManager.UpdateAsync(data);
            if (updatedat == null) return null;
            return user;
        }
    }
}

