using FusionCacheWebApi.Models;

namespace FusionCacheWebApi.Services
{
    public interface IDataService
    {
        Task<string> GetExpensiveDataAsync(string key);
        Task<CacheResponse> SetDataAsync(CacheRequest request);
        Task<CacheResponse> GetDataAsync(string key);
    }
}
