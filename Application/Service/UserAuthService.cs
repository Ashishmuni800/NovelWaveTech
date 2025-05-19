using Application.DTO;
using Application.ServiceInterface;
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
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IMapper _Mapp;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserAuthService(IUserAuthRepository userAuthRepository, IMapper Mapp, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager )
        {
            _userAuthRepository = userAuthRepository;
            _Mapp = Mapp;
            _signInManager = signInManager;
            _userManager = userManager;
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

        public async Task<RegisterDTO> FindByEmailAsync(string Email)
        {
            var model = _Mapp.Map<RegisterModel>(Email);
            var result=await _userAuthRepository.FindByEmailAsync(Email).ConfigureAwait(false);
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

        public Task<LoginDTO> LoginAsync(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var model = _Mapp.Map<RegisterModel>(registerDTO);
            await _userAuthRepository.RegisterAsync(model).ConfigureAwait(false);
            var dto = _Mapp.Map<RegisterDTO>(model);
            return dto;
        }

        public void SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
