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
    public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, AuthenticationResponseDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;

        public RegisterAccountCommandHandler(IAccountRepository accountRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<AuthenticationResponseDto> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
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
           
            var token = _tokenProvider.GenerateToken(account.Id, account.UserName, account.Role);

            await _accountRepository.SaveChangesAsync();

            return new AuthenticationResponseDto(account.UserName, account.Email, token);
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
