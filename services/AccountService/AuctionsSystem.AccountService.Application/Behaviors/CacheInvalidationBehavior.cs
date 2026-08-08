using AuctionsSystem.AccountService.Application.Abstractions.Cache;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Behaviors
{
    public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;
        private readonly ResiliencePipeline _retryPipeline;

        public CacheInvalidationBehavior(IDistributedCache cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger, ResiliencePipelineProvider<string> pipelineProvider)
        {
            _cache = cache;
            _logger = logger;
            _retryPipeline = pipelineProvider.GetPipeline("redis-retry");
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (request is not ICacheInvalidator cacheInvalidator)
            {
                return response;
            }

            var cacheKey = cacheInvalidator.CacheKey;

            try
            {
                await _retryPipeline.ExecuteAsync(async ct =>
                {
                    _logger.LogInformation("[CACHE INVALIDATION] Attempting to remove key: {CacheKey}", cacheKey);
                    await _cache.RemoveAsync(cacheKey, ct);
                }, cancellationToken);

                _logger.LogInformation("[CACHE INVALIDATION] Successfully removed key: {CacheKey}", cacheKey);
            } catch (Exception ex)
            {
                _logger.LogWarning(ex, "[CACHE INVALIDATION] Failed to remove key: {CacheKey}. Database was updated successfully.", cacheKey);
            }

            return response;
        }
    }
}
