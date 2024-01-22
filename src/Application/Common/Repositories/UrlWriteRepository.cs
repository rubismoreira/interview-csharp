using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Common.Repositories;

public class UrlWriteRepository : IUrlWriteRepository
{
    private readonly IApplicationDbContext _context;

    public UrlWriteRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OneOf<Domain.Entities.Url, Error<string>>> AddUrlAsync(Domain.Entities.Url request,
        CancellationToken cancellationToken)
    {
        try
        {
            var generatedUrl = await _context.Urls.AddAsync(request, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return generatedUrl.Entity;
        }
        catch (DbUpdateException e)
        {
            return new Error<string>(e.InnerException?.Message ?? "Unkonwn error");
        }
    }
}
