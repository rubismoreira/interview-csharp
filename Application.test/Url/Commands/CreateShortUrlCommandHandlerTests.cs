using HashidsNet;
using NSubstitute;
using UrlShortenerService.Application.Common.Repositories;
using UrlShortenerService.Application.Url.Commands;
using Shouldly;

namespace Application.test.Url.Commands;

public class CreateShortUrlCommandHandlerTests
{
    private CreateShortUrlCommandHandler _sut;
    private IUrlWriteRepository _urlRepositoryMock;
    private IHashids _hashidsMock;

    public CreateShortUrlCommandHandlerTests()
    {
        _urlRepositoryMock = Substitute.For<IUrlWriteRepository>();

        _urlRepositoryMock.AddUrlAsync(Arg.Any<UrlShortenerService.Domain.Entities.Url>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new UrlShortenerService.Domain.Entities.Url() { Id = 1 });

        _hashidsMock = Substitute.For<IHashids>();

        _sut = new CreateShortUrlCommandHandler(_hashidsMock, _urlRepositoryMock);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsGeneratedHash()
    {
        var result = await _sut.Handle(new CreateShortUrlCommand() { Url = "www.google.com", UserId = "tester" },
            CancellationToken.None);

        result.IsT0.ShouldBeTrue();
        _hashidsMock.Received(1).EncodeLong(Arg.Any<long>());
        await _urlRepositoryMock.Received(1).AddUrlAsync(Arg.Any<UrlShortenerService.Domain.Entities.Url>(),
            Arg.Any<CancellationToken>());
    }
}
