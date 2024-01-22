using FluentValidation;
using HashidsNet;
using MediatR;
using OneOf;
using OneOf.Types;
using UrlShortenerService.Application.Common.Repositories;

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
    private readonly IUrlRepository _urlRepository;
    private readonly IHashids _iHashids;

    public RedirectToUrlCommandHandler(IUrlRepository urlRepository, IHashids iHashids)
    {
        _urlRepository = urlRepository;
        _iHashids = iHashids;
    }

    public async Task<OneOf<string, NotFound>> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var decodedId = _iHashids.DecodeSingleLong(request.Id);
        var url = await _urlRepository.GetUrlByShortUrlAsync(decodedId);
        if (url.IsT1)
        {
            return url.AsT1;
        }

        return url.AsT0.OriginalUrl;
    }
}
