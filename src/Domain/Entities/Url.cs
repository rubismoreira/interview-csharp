using UrlShortenerService.Domain.Common;

namespace UrlShortenerService.Domain.Entities;

/// <summary>
/// Url domain entity.
/// </summary>
public class Url : BaseAuditableEntity
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Url() { }
    
    /// <summary>
    /// The original url.
    /// </summary>
    public string OriginalUrl { get; set; } = default!;
    
    public string ShortUrl { get; set; } = default!;
}
