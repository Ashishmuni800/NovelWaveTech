using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterface
{
    public interface ITransactionsRepository
    {
        Task<Transactions> CreateTransactionsAsync(Transactions Transactions);
        Task<Transactions> GetTransactionsBycustomerIdAsync(Guid customerId);
        Task<Transactions> GetTransactionsByIdAsync(Guid Id);
        Task<IEnumerable<Transactions>> GetAllTransactionsAsync();
        Task<Transactions> UpdateTransactionsAsync(Transactions Transactions, Guid customerId, Guid Id);
        Task<bool> DeleteTransactionsAsync(Guid customerId, Guid Id);
    }
}
