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
    }
}
