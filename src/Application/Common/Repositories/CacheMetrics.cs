using System.Diagnostics.Metrics;

namespace UrlShortenerService.Application.Common.Repositories;

public class CacheMetrics
{
    private readonly Counter<int> _cacheHitCounter;
    private readonly Counter<int> _cacheMissCounter;
    
    public CacheMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("UrlShortenerService.Cache");
        _cacheHitCounter = meter.CreateCounter<int>("cache.hit");
        _cacheMissCounter = meter.CreateCounter<int>("cache.miss");
    }
    
    public void Hit() => _cacheHitCounter.Add(1);
    public void Miss() => _cacheMissCounter.Add(1);
}
