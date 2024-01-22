using OneOf;
using OneOf.Types;

namespace UrlShortenerService.Application.Common.Repositories;

public interface IUrlWriteRepository
{
    public Task<OneOf<Domain.Entities.Url, Error<string>>> AddUrlAsync(Domain.Entities.Url request,
        CancellationToken cancellationToken);
}
