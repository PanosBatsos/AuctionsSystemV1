using AuctionsSystem.AccountService.Application.Events.Logout;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;


namespace AuctionsSystem.AccountService.Application.Events.RevokeToken
{
    public class RevokeTokenEventHandler : INotificationHandler<RevokeTokenEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RevokeTokenEventHandler> _logger;

        public RevokeTokenEventHandler(IDistributedCache cache, ILogger<RevokeTokenEventHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task Handle(RevokeTokenEvent notification, CancellationToken cancellationToken)
        {
            var blacklistKey = $"blacklist-{notification.TokenId}";

            _logger.LogInformation("[SECURITY] Adding token {TokenId} to blacklist", notification.TokenId);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) 
            };

            await _cache.SetStringAsync(blacklistKey, "revoked", options, cancellationToken);

            _logger.LogInformation("[SECURITY] Token successfully blacklisted.");
        }
    }
}
