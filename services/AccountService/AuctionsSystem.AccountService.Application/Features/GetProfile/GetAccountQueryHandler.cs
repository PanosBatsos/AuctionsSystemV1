using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.GetProfile
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, GetAccountQueryResponseDto>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<GetAccountQueryResponseDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.Id);

            if (account == null)
            {
                throw new AccountNotFoundException();
            }

            return new GetAccountQueryResponseDto(account.Id,
                account.UserName,
                account.Email,
                account.FirstName,
                account.LastName,
                account.PhoneNumber,
                account.IdNumber,
                account.Role);        
        }


        
    }
}
