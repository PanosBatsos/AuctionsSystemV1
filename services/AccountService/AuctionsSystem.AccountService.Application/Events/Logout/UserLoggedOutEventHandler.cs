using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Polly;

namespace AuctionsSystem.AccountService.Application.Events.Logout
{
    public class UserLoggedOutEventHandler : INotificationHandler<UserLoggedOutEvent>
    {

        private readonly IDistributedCache _cache;
        private readonly ILogger<UserLoggedOutEventHandler> _logger;
        private readonly ResiliencePipeline _retryPipeline;

        public UserLoggedOutEventHandler(IDistributedCache cache, ILogger<UserLoggedOutEventHandler> logger, ResiliencePipelineProvider<string> pipelineProvider)
        {
            _cache = cache;
            _logger = logger;
            _retryPipeline = pipelineProvider.GetPipeline("redis-retry");
        }

        public async Task Handle(UserLoggedOutEvent notification, CancellationToken cancellationToken)
        {
            var cacheKey = $"account-{notification.Id}";

            await _retryPipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("[CACHE INVALIDATION] Attempting to clear cache for account: {CacheKey}", cacheKey);

                await _cache.RemoveAsync(cacheKey, ct);

            }, cancellationToken);

            _logger.LogInformation("[CACHE INVALIDATION] Successfully removed cache for key: {CacheKey}", cacheKey);
        }
    }
}
