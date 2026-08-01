using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Domain.Entities;
using AuctionsSystem.AccountService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.RegisterAccount
{
    public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        public RegisterAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Guid> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByUniqueFieldsAsync(request.Email,
                request.PhoneNumber,
                request.IdNumber
                );

            ResolveConflicts(existingAccount, request);

            Account account = Account.CreateInitialAccount(request.Username,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.IdNumber);

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            return account.Id;
        }


        private void ResolveConflicts(Account? account, RegisterAccountCommand request)
        {
            if (account == null) return;

            if(account.Email == request.Email)
                throw new ConflictException("Email", "Email already exists");

            if (account.PhoneNumber == request.PhoneNumber)
                throw new ConflictException("PhoneNumber", "Phone number already exists");

            if (account.IdNumber == request.IdNumber)
                throw new ConflictException("IdNumber", $"Id number already exists");
        }
    }
}
