using AuctionsSystem.AccountService.Application.Abstractions.Cache;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuctionsSystem.AccountService.Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheable cacheable) 
            {
                return await next();
            }

            var cacheKey = cacheable.CacheKey;
            _logger.LogInformation("[CACHE] Checking cache for key: {CacheKey}", cacheKey);

            var cacheResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                _logger.LogInformation("[CACHE HIT] Data successfully retrieved from cache for key: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cacheResponse)!;
            }

            _logger.LogInformation("[CACHE MISS] Data not found in cache. Fetching from database for key: {CacheKey}", cacheKey);

            var response = await next();

            _logger.LogInformation("[CACHE] Data fetched from database for key: {CacheKey}. Proceeding to update cache...", cacheKey);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheable.Expiration ?? TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);

            _logger.LogInformation("[CACHE SAVED] Data successfully saved to cache for key: {CacheKey}", cacheKey);

            return response;
        }
    }
}
