using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Auth;
using UrlShortenerService.Api.Endpoints.Url.Requests;
using UrlShortenerService.Application.Url.Commands;
using IMapper = AutoMapper.IMapper;

namespace Api.Endpoints.Url;
 
public class CreateShortUrlSummary : Summary<CreateShortUrlEndpoint>
{
    public CreateShortUrlSummary()
    {
        Summary = "Create short url from provided url";
        Description =
            "This endpoint will create a short url from provided original url.";
        Response(500, "Internal server error.");
    }
}

public class CreateShortUrlEndpoint : BaseEndpoint<CreateShortUrlRequest>
{
    public CreateShortUrlEndpoint(ISender mediator, IMapper mapper)
        : base(mediator, mapper) { }

    public override void Configure()
    {
        base.Configure();
        Post("u");
        Policies(AuthData.WriterPolicy);
        // AllowAnonymous();
        Description(
            d => d.WithTags("Url")
        );
        Summary(new CreateShortUrlSummary());
    }

    public override async Task HandleAsync(CreateShortUrlRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User?.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
        
        var command = Mapper.Map<CreateShortUrlCommand>(req, 
            opts => opts.Items["UserId"] = userId);
            
        var result = await Mediator.Send(
            command,
            ct
        );
        
        if (result.IsT1)
        {
            await SendAsync(result.AsT1, StatusCodes.Status400BadRequest);
            return;
        }
        
        await SendOkAsync(result.AsT0);
    }
}
