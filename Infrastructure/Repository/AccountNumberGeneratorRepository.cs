using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AccountNumberGeneratorRepository : IAccountNumberGeneratorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        //private const string DefaultStart = "10000000001";
        public AccountNumberGeneratorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> GenerateNextAccountNumberAsync(int numericLength = 11)
        {
            const int maxAttempts = 10;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                string numericPart = GenerateSecureNumeric(numericLength);
                string accountNumber = $"{numericPart}";

                bool exists = await _dbContext.Customers
                    .AnyAsync(c => c.AccountNumber == accountNumber);

                if (!exists)
                    return accountNumber;
            }

            throw new Exception("Failed to generate a unique account number after multiple attempts.");

            static string GenerateSecureNumeric(int length = 11, bool firstNonZero = true)
            {
                if (length <= 0)
                    throw new ArgumentException("Length must be > 0", nameof(length));

                var sb = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    int min = (i == 0 && firstNonZero) ? 1 : 0;
                    int digit = RandomNumberGenerator.GetInt32(min, 10);
                    sb.Append((char)('0' + digit));
                }

                return sb.ToString();
            }
        }
    }
    }
