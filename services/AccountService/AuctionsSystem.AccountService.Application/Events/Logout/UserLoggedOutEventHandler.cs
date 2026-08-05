using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Events.Logout
{
    public class UserLoggedOutEventHandler : INotificationHandler<UserLoggedOutEvent>
    {

        private readonly IDistributedCache _cache;
        private readonly ILogger<UserLoggedOutEventHandler> _logger;

        public UserLoggedOutEventHandler(IDistributedCache cache, ILogger<UserLoggedOutEventHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task Handle(UserLoggedOutEvent notification, CancellationToken cancellationToken)
        {
            var cacheKey = $"account-{notification.Id}";

            _logger.LogInformation("[CACHE INVALIDATION] Started clearing cache for account on logout: {CacheKey}", cacheKey);

            await _cache.RemoveAsync(cacheKey, cancellationToken);

            _logger.LogInformation("[CACHE INVALIDATION] Successfully removed cache for key: {CacheKey}", cacheKey);
        }
    }
}
