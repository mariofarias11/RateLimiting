using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace RateLimiting.Host.Cache
{
    public class RateLimitVerifier : IRateLimitVerifier
    {
        private static MemoryCache _memoryCache { get; set; }  
        private readonly ILogger<RateLimitVerifier> _logger;

        public RateLimitVerifier(ILogger<RateLimitVerifier> logger)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _logger = logger;
        }

        public bool Verify(HttpContext context, string requestName, int seconds, int limitRequests)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            var memoryCacheKey = requestName + "-" + ipAddress;
            
            var knownRequestor = _memoryCache.TryGetValue(memoryCacheKey, out int attempts);

            if (!knownRequestor)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds));
                _memoryCache.Set(memoryCacheKey, 1, cacheEntryOptions);
            }
            else
            {
                if(attempts < limitRequests)
                {
                    _memoryCache.Set(memoryCacheKey, attempts + 1);
                }
                else
                {
                    _logger.LogWarning($"Rate limit exceeded. Current attempt request: {attempts}");
                    return false;
                }
            }

            return true;
        }
    }
}
