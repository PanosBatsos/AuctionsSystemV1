using AuctionsSystem.AccountService.Application.Events.Logout;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;


namespace AuctionsSystem.AccountService.Application.Events.RevokeToken
{
    public class RevokeTokenEventHandler : INotificationHandler<RevokeTokenEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RevokeTokenEventHandler> _logger;
        private readonly ResiliencePipeline _retryPipeline;

        public RevokeTokenEventHandler(IDistributedCache cache, ILogger<RevokeTokenEventHandler> logger, ResiliencePipelineProvider<string> pipelineProvider)
        {
            _cache = cache;
            _logger = logger;
            _retryPipeline = pipelineProvider.GetPipeline("redis-retry");
        }
        public async Task Handle(RevokeTokenEvent notification, CancellationToken cancellationToken)
        {
            var blacklistKey = $"blacklist-{notification.TokenId}";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };

            await _retryPipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("[SECURITY] Attempting to blacklist token {TokenId}", notification.TokenId);

                await _cache.SetStringAsync(blacklistKey, "revoked", options, ct);

            }, cancellationToken);

            _logger.LogInformation("[SECURITY] Token successfully blacklisted.");
        }
    }
}
