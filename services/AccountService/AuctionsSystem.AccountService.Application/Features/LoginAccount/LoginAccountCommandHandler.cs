using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.LoginAccount
{
    public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, AuthenticationResponseDto>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;

        public LoginAccountCommandHandler(ITokenProvider tokenProvider, IAccountRepository accountRepository, IPasswordHasher passwordHasher)
        {
            _tokenProvider = tokenProvider;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticationResponseDto> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);

            CheckAccountDetails(request, account);

            account.RecordLoginSuccess("Ip");
            
        }

        private void CheckAccountDetails(LoginAccountCommand request, Account? account)
        {
            if (account == null)
                throw new Exception(); // AccountDoesNotExistsException

            if (!_passwordHasher.Verify(request.Password, account.Security.PasswordHash))
            {
                account.RecordUnsuccesfulLoginTry(request.IpAddress);
                throw new Exception(); // PasswordMismatchException
            }

            if (!account.IsActive)
            {
                account.RecordUnsuccesfulLoginTry(request.IpAddress);
                throw new Exception(); // InactiveAccountException
            }

            if (!account.Verification.IsVerified())
            {
                account.RecordUnsuccesfulLoginTry(request.IpAddress);
                throw new Exception(); // NotVerifiedAccountException
            }

            if (!account.Security.IsLockedOut())
            {
                account.RecordUnsuccesfulLoginTry(request.IpAddress);
                throw new Exception(); // AccountLockedOutException
            }
        }
    }
}
