using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface IServiceInfraRepo
    {
        IUserAuthRepository AuthRepo { get; set; }
        IProductRepository ProductRepo { get; set; }
        ITransactionsRepository TransactionsRepo { get; set; }
        IRemindersRepository RemindersRepo { get; set; }
        ICustomerRepository CustomerRepo { get; set; }
        IAccountNumberGeneratorRepository AccountNumberGeneratorRepo { get; set; }
    }
}
