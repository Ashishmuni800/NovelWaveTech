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

        public async Task<List<CustomerBalance>> GetBalanceBycustomerIdAsync(Guid customerId)
        {
            var result = await (
                from c in _dbContext.Customers
                join t in _dbContext.Transactions
                    on c.Id equals t.CustomerId
                where c.Id == customerId
                group t by new
                {
                    c.Id,
                    c.Name
                }
                into g
                select new CustomerBalance
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    TotalCredit = g.Where(x => x.Type == TransactionType.Credit)
                                   .Sum(x => x.Amount),
                    TotalDebit = g.Where(x => x.Type == TransactionType.Debit)
                                  .Sum(x => x.Amount)
                }
            ).ToListAsync();

            return result ?? new List<CustomerBalance>();
        }

        public async Task<List<CustomerBalance>> GetBalanceAsync()
        {
            return await _dbContext.Transactions
                .GroupBy(t => new
                {
                    t.Customer.Id,
                    t.Customer.Name
                })
                .Select(g => new CustomerBalance
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    TotalCredit = g.Where(x => x.Type == TransactionType.Credit)
                                   .Sum(x => x.Amount),
                    TotalDebit = g.Where(x => x.Type == TransactionType.Debit)
                                  .Sum(x => x.Amount)
                })
                .ToListAsync();
        }
        public async Task<List<CustomerBalance>> GetTotalBalance()
        {
            List<CustomerBalance> items = new List<CustomerBalance>();
            var transactions = await _dbContext.Transactions
                .ToListAsync();

            var totalCredit = transactions
                .Where(t => t.Type == TransactionType.Credit)
                .Sum(t => t.Amount);

            var totalDebit = transactions
                .Where(t => t.Type == TransactionType.Debit)
                .Sum(t => t.Amount);

            items.Add(new CustomerBalance
            {
                TotalCredit = totalCredit,
                TotalDebit = totalDebit
            });

            return items;
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
