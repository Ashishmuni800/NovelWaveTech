using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface IServiceInfra
    {
        IUserAuthService AuthService { get; set; }
        IProductServices ProductService { get; set; }
        ICustomerService CustomerService { get; set; }
        ITransactionsService TransactionsService { get; set; }
        IRemindersService RemindersService { get; set; }
        IAccountNumberGenerator AccountNumberGenerator { get; set; }
        IGenerateRandomCaptchaCode GenerateRandomCaptchaCode { get; set; }
    }
}
