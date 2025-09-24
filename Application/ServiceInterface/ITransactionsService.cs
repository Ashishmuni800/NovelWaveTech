using Application.DTO;
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
        Task<TransactionDTO> GetTransactionBycustomerIdAsync(Guid customerId);
        Task<TransactionDTO> GetTransactionByIdAsync(Guid Id);
        Task<IEnumerable<TransactionDTO>> GetAllTransactionAsync();
        Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO, Guid customerId, Guid Id);
        Task<bool> DeleteTransactionAsync(Guid customerId, Guid Id);
    }
}
