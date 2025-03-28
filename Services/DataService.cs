using FusionCacheWebApi.Models;
using ZiggyCreatures.Caching.Fusion;

namespace FusionCacheWebApi.Services
{
    public class DataService : IDataService
    {
        private readonly IFusionCache _cache;

        public DataService(IFusionCache cache)
        {
            _cache = cache;
        }

        public async Task<string> GetExpensiveDataAsync(string key)
        {
            return await _cache.GetOrSetAsync<string>(
                key: $"data:{key}",
                factory: async (ctx, cancellationToken) =>
                {
                    await Task.Delay(1000);
                    return $"Data for {key} generated at {DateTime.UtcNow}";
                },
                options: new FusionCacheEntryOptions()
                    .SetDuration(TimeSpan.FromMinutes(1))
                    .SetFailSafe(true, TimeSpan.FromHours(1))
            );
        }

        public async Task<CacheResponse> SetDataAsync(CacheRequest request)
        {
            await _cache.SetAsync(
                   request.Key,
                   request.Value,
                   new FusionCacheEntryOptions
                   {
                       Duration = request.Expiration,
                       IsFailSafeEnabled = true
                   }
               );

            var response = new CacheResponse
            {
                Status = request.Value,
                Key = request.Key,
                ExpiresIn = $"{request.Expiration.TotalMinutes} mins"
            };

            return response;
        }


        public async Task<CacheResponse> GetDataAsync(string key)
        {
            // Get value
            var cachedValue = await _cache.GetOrDefaultAsync<string>(key);

            var value = $"cached_at_{DateTime.UtcNow}";

            var response = new CacheResponse
            {
                Status = cachedValue,
                Key = key,
                ExpiresIn = value               
            };

            return response;
        }

    }
}
