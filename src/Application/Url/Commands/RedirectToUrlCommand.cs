using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Interfaces;
using OneOf;
using OneOf.Types;

namespace UrlShortenerService.Application.Url.Commands;

public record RedirectToUrlCommand : IRequest<OneOf<string, NotFound>>
{
    public string Id { get; init; } = default!;
}

public class RedirectToUrlCommandValidator : AbstractValidator<RedirectToUrlCommand>
{
    public RedirectToUrlCommandValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

public class RedirectToUrlCommandHandler : IRequestHandler<RedirectToUrlCommand, OneOf<string, NotFound>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public RedirectToUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<OneOf<string, NotFound>> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(x => x.ShortUrl == request.Id, cancellationToken);
        if (url == null)
        {
            return new NotFound();
        }

        return url.OriginalUrl;
    }
}
