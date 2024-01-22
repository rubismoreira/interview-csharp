using OneOf;
using OneOf.Types;

namespace UrlShortenerService.Application.Common.Repositories;

public interface IUrlRepository
{
    public Task<OneOf<Domain.Entities.Url, NotFound>> GetUrlByShortUrlAsync(long id);
}
