using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Domain.Entities;
using AuctionsSystem.AccountService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.RegisterAccount
{
    public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, RegisterAccountCommandResponseDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterAccountCommandHandler(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterAccountCommandResponseDto> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByUniqueFieldsAsync(request.Email,
                request.PhoneNumber,
                request.IdNumber
                );

            ResolveConflicts(existingAccount, request);

            string hashedPassword = _passwordHasher.Hash(request.Password);


            Account account = Account.CreateInitialAccount(request.Username,
                request.Email,
                hashedPassword,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.IdNumber);

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            return new RegisterAccountCommandResponseDto(account.Id, account.Email, account.PhoneNumber);
        }


        private void ResolveConflicts(Account? account, RegisterAccountCommand request)
        {
            if (account == null) return;

            if (account.UserName == request.Username)
            {
                throw new PropertyAlreadyInUseException("Usename", "Username already exists");
            }

            if(account.Email == request.Email)
                throw new PropertyAlreadyInUseException("Email", "Email already exists");

            if (account.PhoneNumber == request.PhoneNumber)
                throw new PropertyAlreadyInUseException("PhoneNumber", "Phone number already exists");

            if (account.IdNumber == request.IdNumber)
                throw new PropertyAlreadyInUseException("IdNumber", "Id number already exists");
            
        }
    }
}
