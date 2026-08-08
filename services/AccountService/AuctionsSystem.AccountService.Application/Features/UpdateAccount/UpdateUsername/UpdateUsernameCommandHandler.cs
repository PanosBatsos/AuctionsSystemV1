using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername
{
    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public UpdateUsernameCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);
            if (account == null)
            {
                throw new AccountNotFoundException();
            }

            if (account.UserName.Equals(request.NewUsername)) 
            {
                return;
            }

            var isTaken = await _accountRepository.IsUsernameTakenAsync(request.NewUsername, cancellationToken);
            if (isTaken)
            {
                throw new PropertyAlreadyInUseException("Username", "Username already exists");
            }

            account.ChangeUsername(request.NewUsername);

            await _accountRepository.UpdateAsync(account, cancellationToken);
        }
    }
}
