using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace UrlShortenerService.Application.Common.Repositories;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly CacheMetrics _cacheMetrics;
    
    public RedisCacheService(IDistributedCache distributedCache, CacheMetrics cacheMetrics)
    {
        _distributedCache = distributedCache;
        _cacheMetrics = cacheMetrics;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };
        var jsonData = JsonSerializer.Serialize(value);
        _distributedCache.SetString(key, jsonData, options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _distributedCache.GetStringAsync(key);
        if (jsonData == null)
        {
            _cacheMetrics.Miss();
            return default(T);
        }
        
        _cacheMetrics.Hit();
        return JsonSerializer.Deserialize<T>(jsonData);
    }
}
