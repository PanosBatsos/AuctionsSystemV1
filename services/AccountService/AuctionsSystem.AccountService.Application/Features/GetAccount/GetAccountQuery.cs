using AuctionsSystem.AccountService.Application.Abstractions.Cache;
using AuctionsSystem.AccountService.Application.Abstractions.Logging;
using AuctionsSystem.AccountService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.GetAccount
{
    public record GetAccountQuery(Guid Id) : IRequest<GetAccountQueryResponseDto>, ILoggableRequest, ICacheable
    {
        public string CacheKey => $"account-{Id}";

        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
