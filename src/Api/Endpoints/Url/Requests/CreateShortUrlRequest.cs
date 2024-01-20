using Api.Endpoints.Url;

namespace UrlShortenerService.Api.Endpoints.Url.Requests;

/// <summary>
/// Request model for the <see cref="CreateShortUrlEndpoint"/>.
/// </summary>
public class CreateShortUrlRequest
{
    /// <summary>
    /// The long url to shorten.
    /// </summary>
    public string Url { get; set; } = default!;
}
