namespace UrlShortenerService.Application.Common.Repositories;

public interface IRedisCacheService
{
    public Task SetAsync<T>(string key, T value, TimeSpan expiry);
    public Task<T?> GetAsync<T>(string key);
}
