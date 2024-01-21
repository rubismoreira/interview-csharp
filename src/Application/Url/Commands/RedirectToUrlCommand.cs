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

    public RedirectToUrlCommandHandler(IUrlRepository urlRepository)
    {
        _urlRepository = urlRepository;
    }

    public async Task<OneOf<string, NotFound>> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var url = await _urlRepository.GetUrlByShortUrlAsync(request.Id);
        if (url.IsT1)
        {
            return url.AsT1;
        }

        return url.AsT0.OriginalUrl;
    }
}
