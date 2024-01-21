using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using UrlShortenerService.Application.Common.Interfaces;
using UrlShortenerService.Domain.Options;

namespace UrlShortenerService.Application.Common.Repositories;

public class UrlRepository : IUrlRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IApplicationDbContext _context;
    private readonly CacheOptions _cacheOptions;
    private static string _cachePrefix = "url-";

    public UrlRepository(IRedisCacheService redisCacheService, 
        IApplicationDbContext context,
        IOptions<CacheOptions> options)
    {
        _redisCacheService = redisCacheService;
        _context = context;
        _cacheOptions = options.Value;
    }

    private async Task<OneOf<Domain.Entities.Url, NotFound>> GetFromCache(string shortUrl)
    {
        var url = await _redisCacheService.GetAsync<Domain.Entities.Url>(_cachePrefix + shortUrl);
        if (url is not null)
        {
            return url;
        }

        var originCall = await GetFromOrigin(shortUrl);
        
        if (originCall.IsT0)
        {
            await _redisCacheService.SetAsync(_cachePrefix + shortUrl, originCall.AsT0, TimeSpan.FromMinutes(5));
            return originCall.AsT0;
        }

        return new NotFound();
    }

    private async Task<OneOf<Domain.Entities.Url, NotFound>> GetFromOrigin(string shortUrl)
    {
        Domain.Entities.Url? url;
        url = await _context.Urls.FirstOrDefaultAsync(x => x.ShortUrl == shortUrl);
        
        if(url is not null)
        {
            return url;
        }

        return new NotFound();
    }
    
    public Task<OneOf<Domain.Entities.Url, NotFound>> GetUrlByShortUrlAsync(string shortUrl)
    {
        if (_cacheOptions.UseRedis)
            return GetFromCache(shortUrl);

        return GetFromOrigin(shortUrl);
    }
}
