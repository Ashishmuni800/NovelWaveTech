using Application.DTO;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface ITransactionsService
    {
        Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO);
        Task<List<TransactionViewModel>> GetTransactionBycustomerIdAsync(Guid customerId);
        Task<CustomerBalanceDTO> GetBalanceBycustomerIdAsync(Guid customerId);
        Task<TransactionViewModel> GetTransactionByIdAsync(Guid Id);
        Task<IEnumerable<TransactionViewModel>> GetAllTransactionAsync();
        Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO, Guid customerId, Guid Id);
        Task<bool> DeleteTransactionAsync(Guid customerId, Guid Id);
    }
}
