using Microsoft.Extensions.Caching.Memory;
using QualityControlApp.Models.Entities;
using System.Text.Json;

namespace QualityControlApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(15);

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out T? cachedValue))
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(5) // Reset expiration if accessed
                };

                _memoryCache.Set(key, value, cacheOptions);
                _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration ?? _defaultExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Removed cache entry for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: IMemoryCache doesn't support pattern-based removal
                // In a production environment, consider using Redis or another distributed cache
                _logger.LogWarning("Pattern-based cache removal not supported with IMemoryCache. Pattern: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return _memoryCache.TryGetValue(key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task<long> IncrementAsync(string key, long value = 1)
        {
            try
            {
                var cachedValue = _memoryCache.Get<long?>(key);
                var currentValue = cachedValue ?? 0;
                var newValue = currentValue + value;
                _memoryCache.Set(key, newValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _defaultExpiration,
                    Priority = CacheItemPriority.Normal
                });
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing cache value for key: {Key}", key);
                return 0;
            }
        }

        public async Task<long> DecrementAsync(string key, long value = 1)
        {
            try
            {
                var cachedValue = _memoryCache.Get<long?>(key);
                var currentValue = cachedValue ?? 0;
                var newValue = Math.Max(0, currentValue - value);
                _memoryCache.Set(key, newValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _defaultExpiration,
                    Priority = CacheItemPriority.Normal
                });
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrementing cache value for key: {Key}", key);
                return 0;
            }
        }

        // Landing-specific cache methods
        public async Task<IEnumerable<Landing>?> GetLandingsAsync(string cacheKey)
        {
            return await GetAsync<IEnumerable<Landing>>(cacheKey);
        }

        public async Task SetLandingsAsync(string cacheKey, IEnumerable<Landing> landings, TimeSpan? expiration = null)
        {
            await SetAsync(cacheKey, landings.ToList(), expiration);
        }

        public async Task InvalidateLandingCacheAsync()
        {
            try
            {
                // Remove common landing cache keys
                var keysToRemove = new[]
                {
                    "landings_all",
                    "landings_pending",
                    "landings_approved",
                    "landings_rejected",
                    "landings_stats",
                    "landings_operators",
                    "landings_monthly"
                };

                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key);
                }

                _logger.LogInformation("Invalidated all landing cache entries");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating landing cache");
            }
        }

        public async Task<object?> GetLandingStatsAsync()
        {
            return await GetAsync<object>("landings_stats");
        }

        public async Task SetLandingStatsAsync(object stats, TimeSpan? expiration = null)
        {
            await SetAsync("landings_stats", stats, expiration);
        }

        // Helper methods for generating cache keys
        public static string GetLandingsCacheKey(string? operatorName = null, string? aircraftReg = null, 
            DateTime? flightDateFrom = null, DateTime? flightDateTo = null, string? requestStatus = null)
        {
            var keyParts = new List<string> { "landings" };
            
            if (!string.IsNullOrEmpty(operatorName))
                keyParts.Add($"op_{operatorName}");
            
            if (!string.IsNullOrEmpty(aircraftReg))
                keyParts.Add($"ac_{aircraftReg}");
            
            if (flightDateFrom.HasValue)
                keyParts.Add($"from_{flightDateFrom.Value:yyyyMMdd}");
            
            if (flightDateTo.HasValue)
                keyParts.Add($"to_{flightDateTo.Value:yyyyMMdd}");
            
            if (!string.IsNullOrEmpty(requestStatus))
                keyParts.Add($"status_{requestStatus}");
            
            return string.Join("_", keyParts);
        }

        public static string GetLandingStatsCacheKey()
        {
            return "landings_stats";
        }

        public static string GetLandingOperatorsCacheKey()
        {
            return "landings_operators";
        }

        public static string GetLandingMonthlyCacheKey()
        {
            return "landings_monthly";
        }
    }
}
