using FluentValidation;
using HashidsNet;
using MediatR;
using OneOf;
using OneOf.Types;
using UrlShortenerService.Application.Common.Repositories;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<OneOf<string, Error<string>>>
{
    public string Url { get; init; } = default!;
    public string UserId { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
            .NotEmpty()
            .WithMessage("Url is required.")
            .Must(ValidUrl)
            .WithMessage("Url is not valid.");
    }

    private bool ValidUrl(string input)
    {
        if (input.StartsWith("www"))
        {
            input = "http://" + input;
        }

        return Uri.IsWellFormedUriString(input, UriKind.Absolute);
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, OneOf<string, Error<string>>>
{
    private readonly IUrlWriteRepository _repository;
    private readonly IHashids _hashids;

    public CreateShortUrlCommandHandler(IHashids hashids, IUrlWriteRepository repository)
    {
        _hashids = hashids;
        _repository = repository;
    }

    public async Task<OneOf<string, Error<string>>> Handle(CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        var generatedUrl = await _repository.AddUrlAsync(
            new UrlShortenerService.Domain.Entities.Url { OriginalUrl = request.Url, CreatedBy = request.UserId },
            cancellationToken);

        if (generatedUrl.IsT1)
            return generatedUrl.AsT1;

        return _hashids.EncodeLong(generatedUrl.AsT0.Id);
    }
}
