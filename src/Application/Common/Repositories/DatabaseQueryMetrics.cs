using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace UrlShortenerService.Application.Common.Repositories;

public class DatabaseQueryMetrics
{
    private readonly Histogram<double> _queryTimeHistogram;

    public DatabaseQueryMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("UrlShortenerService.Database");
        _queryTimeHistogram = meter.CreateHistogram<double>("database.query.time");
    }
    
    public void RecordQueryTime(string queryName, double milliseconds)
    {
        var tags = new TagList
        {
            { "queryName", queryName }
        };

        _queryTimeHistogram.Record(milliseconds, tags);
    }
}
