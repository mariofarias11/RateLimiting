using Microsoft.AspNetCore.Http;

namespace RateLimiting.Host.Cache
{
    public interface IRateLimitVerifier
    {
        bool Verify(HttpContext context, string requestName = null, int seconds = 3600, int limitRequests = 100);

    }
}
