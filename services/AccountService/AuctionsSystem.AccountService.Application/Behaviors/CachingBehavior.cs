using AuctionsSystem.AccountService.Application.Abstractions.Cache;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
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

        public CachingBehavior(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheable cacheable) 
            {
                return await next();
            }

            var cacheKey = cacheable.CacheKey;

            var cacheResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                return JsonSerializer.Deserialize<TResponse>(cacheResponse)!;
            }

            var response = await next();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheable.Expiration ?? TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);

            return response;
        }
    }
}
