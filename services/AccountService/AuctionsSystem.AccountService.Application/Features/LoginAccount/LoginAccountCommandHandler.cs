using AuctionsSystem.AccountService.Api.ExceptionHandling;
using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Exceptions;
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

            await CheckLoginRequestCredentialsAsync(account, request, cancellationToken);

            CheckAccountStatus(account);
            
            account.RecordLoginSuccess(request.IpAddress);
       
            var token = _tokenProvider.GenerateToken(account.Id, account.UserName, account.Role);
 
            await _accountRepository.SaveChangesAsync();

            return new AuthenticationResponseDto(account.UserName, account.Email, token);
        }


        private async Task CheckLoginRequestCredentialsAsync(Account? account, LoginAccountCommand request, CancellationToken cancellationToken)
        {
            if (account == null)
            {
                throw new InvalidCredentialsException();     
            }

            if (!_passwordHasher.Verify(request.Password, account.Security.PasswordHash))
            {
                account.RecordUnsuccesfulLoginTry(request.IpAddress);
                await _accountRepository.SaveChangesAsync(cancellationToken);
                throw new InvalidCredentialsException();  
            }
        }

        private void CheckAccountStatus(Account account)
        {
            if (account.Security.IsLockedOut())
                throw new LockedOutAccountException();

            if (!account.IsActive)
                throw new InactiveAccountException();

            if (!account.Verification.IsVerified())
                throw new NotVerifiedAccountException();
        }
    }
}
