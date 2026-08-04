using AuctionsSystem.AccountService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Abstractions.Persistence
{
    public interface IAccountRepository
    {

        Task<Account?> GetByUniqueFieldsAsync(string email, string phoneNumber, string idNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Account account, CancellationToken cancellationToken = default);
        Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Account?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
