using System.Diagnostics;
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
    private readonly DatabaseQueryMetrics _databaseQueryMetrics;
    private static string _cachePrefix = "url-";

    public UrlRepository(IRedisCacheService redisCacheService, 
        IApplicationDbContext context,
        IOptions<CacheOptions> options,
        DatabaseQueryMetrics databaseQueryMetrics)
    {
        _redisCacheService = redisCacheService;
        _context = context;
        _cacheOptions = options.Value;
        _databaseQueryMetrics = databaseQueryMetrics;
    }

    private async Task<OneOf<Domain.Entities.Url, NotFound>> GetFromCache(long id)
    {
        var url = await _redisCacheService.GetAsync<Domain.Entities.Url>(_cachePrefix + id);
        if (url is not null)
        {
            return url;
        }

        var originCall = await GetFromOrigin(id);
        
        if (originCall.IsT0)
        {
            await _redisCacheService.SetAsync(_cachePrefix + id, originCall.AsT0, TimeSpan.FromMinutes(5));
            return originCall.AsT0;
        }

        return new NotFound();
    }

    private async Task<OneOf<Domain.Entities.Url, NotFound>> GetFromOrigin(long id)
    {
        var stopwatch = Stopwatch.StartNew(); 
        var url = await _context.Urls.FirstOrDefaultAsync(x => x.Id == id);
        stopwatch.Stop(); 
        _databaseQueryMetrics.RecordQueryTime("GetUrlById", stopwatch.Elapsed.TotalMilliseconds);
        
        if(url is not null)
        {
            return url;
        }

        return new NotFound();
    }
    
    public Task<OneOf<Domain.Entities.Url, NotFound>> GetUrlByShortUrlAsync(long id)
    {
        if (_cacheOptions.UseRedis)
            return GetFromCache(id);

        return GetFromOrigin(id);
    }
}
