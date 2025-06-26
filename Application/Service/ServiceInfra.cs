using Application.ServiceInterface;
using AutoMapper;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ServiceInfra : IServiceInfra
    {
        private readonly ServiceInfraRepo _infra;
        private readonly IMapper _Mapp;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public ServiceInfra(IServiceInfraRepo infra, IMapper Mapp, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _infra = (ServiceInfraRepo?)infra;
            _Mapp = Mapp;
            _signInManager=signInManager;
            _userManager=userManager;
            AuthService = new UserAuthService(_infra, Mapp,_signInManager,_userManager);
            GenerateRandomCaptchaCode= new GenerateRandomCaptchaCode();
        }
        public IUserAuthService AuthService { get; set; }
        public IGenerateRandomCaptchaCode GenerateRandomCaptchaCode { get; set; }
    }
}
