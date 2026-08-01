using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        private readonly AccountDbContext _db;

        public AccountRepository(AccountDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
        {
            await _db.Set<Account>().AddAsync(account, cancellationToken);
        }

        public async Task<Account?> GetByUniqueFieldsAsync(string email, string phoneNumber, string idNumber, CancellationToken cancellationToken = default)
        {
            return await _db.Accounts
                .Where(u => u.Email == email ||
                        u.PhoneNumber == phoneNumber ||
                        u.IdNumber == idNumber)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
