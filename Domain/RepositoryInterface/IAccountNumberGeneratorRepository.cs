using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface IAccountNumberGeneratorRepository
    {
        Task<string> GenerateNextAccountNumberAsync(int numericLength = 11);
    }
}
