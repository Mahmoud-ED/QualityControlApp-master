using QualityControlApp.Models.Entities;

namespace QualityControlApp.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
        Task<long> IncrementAsync(string key, long value = 1);
        Task<long> DecrementAsync(string key, long value = 1);
        
        // Landing-specific cache methods
        Task<IEnumerable<Landing>?> GetLandingsAsync(string cacheKey);
        Task SetLandingsAsync(string cacheKey, IEnumerable<Landing> landings, TimeSpan? expiration = null);
        Task InvalidateLandingCacheAsync();
        Task<object?> GetLandingStatsAsync();
        Task SetLandingStatsAsync(object stats, TimeSpan? expiration = null);
    }
}
