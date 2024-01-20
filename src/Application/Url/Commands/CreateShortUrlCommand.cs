using System.Text;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Interfaces;
using OneOf;
using OneOf.Types;

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
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public CreateShortUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public static string UrlToHex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public async Task<OneOf<string, Error<string>>> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var hexed = UrlToHex(request.Url);
        var encoded = _hashids.EncodeHex(hexed);

        try
        {
            var temp = await _context.Urls.AddAsync(new UrlShortenerService.Domain.Entities.Url
            {
                OriginalUrl = request.Url,
                ShortUrl = encoded,
                CreatedBy = request.UserId
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            return new Error<string>(e.InnerException.Message);
        }
        return encoded;
    }
}
