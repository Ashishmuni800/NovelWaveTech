using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transactions> CreateTransactionsAsync(Transactions Transactions)
        {
            await _dbContext.Transactions.AddAsync(Transactions);
            await _dbContext.SaveChangesAsync();
            return Transactions;
        }

        public async Task<bool> DeleteTransactionsAsync(Guid customerId, Guid id)
        {
            var Transactions = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.CustomerId == customerId);

            if (Transactions == null)
                return false;

            _dbContext.Transactions.Remove(Transactions);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Transactions>> GetAllTransactionsAsync()
        {
            return await _dbContext.Transactions
                .Include(t => t.Customer)
                .ToListAsync();
        }

        public async Task<CustomerBalance> GetBalanceBycustomerIdAsync(Guid customerId)
        {
            var transactions = await _dbContext.Transactions
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            var totalCredit = transactions
                .Where(t => t.Type == TransactionType.Credit)
                .Sum(t => t.Amount);

            var totalDebit = transactions
                .Where(t => t.Type == TransactionType.Debit)
                .Sum(t => t.Amount);

            return new CustomerBalance
            {
                TotalCredit = totalCredit,
                TotalDebit = totalDebit
            };
        }

        public async Task<CustomerBalance> GetBalanceAsync()
        {
            var transactions = await _dbContext.Transactions
                .ToListAsync();

            var totalCredit = transactions
                .Where(t => t.Type == TransactionType.Credit)
                .Sum(t => t.Amount);

            var totalDebit = transactions
                .Where(t => t.Type == TransactionType.Debit)
                .Sum(t => t.Amount);

            return new CustomerBalance
            {
                TotalCredit = totalCredit,
                TotalDebit = totalDebit
            };
        }

        public async Task<List<Transactions>> GetTransactionsBycustomerIdAsync(Guid customerId)
        {
            return await _dbContext.Transactions.Where(op => op.CustomerId == customerId).ToListAsync();
        }

        public async Task<Transactions> GetTransactionsByIdAsync(Guid id)
        {
            return await _dbContext.Transactions
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transactions> UpdateTransactionsAsync(Transactions updatedTransactions, Guid customerId, Guid id)
        {
            var existingTransactions = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.CustomerId == customerId);

            if (existingTransactions == null)
                return null;

            existingTransactions.Amount = updatedTransactions.Amount;
            existingTransactions.Type = updatedTransactions.Type;
            existingTransactions.Notes = updatedTransactions.Notes;
            existingTransactions.TransactionDate = updatedTransactions.TransactionDate;

            await _dbContext.SaveChangesAsync();
            return existingTransactions;
        }
    }

}
