using AutoMapper;
using UrlShortenerService.Api.Endpoints.Url.Requests;
using UrlShortenerService.Application.Url.Commands;

namespace Api.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateShortUrlRequest, CreateShortUrlCommand>()
            .ForMember(dest => dest.UserId, 
                opt => opt.MapFrom(
                    (src, dest, destMember, context) => 
                                    context.Items["UserId"] as string));
    }
}
