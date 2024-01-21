namespace UrlShortenerService.Domain.Options
{
    public class CacheOptions
    {
        public bool UseRedis { get; set; }
        public string RedisServer { get; set; } = string.Empty;
    }
}
