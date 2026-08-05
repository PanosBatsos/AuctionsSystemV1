using AuctionsSystem.AccountService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuctionsSystem.AccountService.Application.Events.Login
{
    public class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<UserLoggedInEventHandler> _logger;

        public UserLoggedInEventHandler(IDistributedCache cache, ILogger<UserLoggedInEventHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
        {
            var account = notification.Account;

            var cacheKey = $"account-{account.Id}";

            _logger.LogInformation("[CACHE WARMING] Started background caching for user: {CacheKey}", cacheKey);

            var accountDto = new GetAccountQueryResponseDto(account.Id,
                account.UserName,
                account.Email,
                account.FirstName,
                account.LastName,
                account.PhoneNumber,
                account.IdNumber,
                account.Role);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) 
            };

            await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(accountDto),
            cacheOptions,
            cancellationToken);

            _logger.LogInformation("[CACHE WARMING] Successfully populated cache for key: {CacheKey}", cacheKey);
        }
    }
}
