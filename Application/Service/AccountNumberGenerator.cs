using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.RepositoryInterface;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Service
{


    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IServiceInfraRepo _infra;
        private readonly IMapper _Mapp;
        public AccountNumberGenerator(IServiceInfraRepo AccountNumberGeneratorService, IMapper Mapp)
        {
            _infra = AccountNumberGeneratorService;
            _Mapp= Mapp;
        }

        public async Task<string> GenerateAccountNumberAsync()
        {
            var result = await _infra.AccountNumberGeneratorRepo.GenerateNextAccountNumberAsync().ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<string>(result);
                return dtoList;
            }

            return null;
        }
    }

}
