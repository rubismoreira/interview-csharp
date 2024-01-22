using FluentValidation.TestHelper;
using UrlShortenerService.Application.Url.Commands;

namespace Application.test.Url.Commands;

public class CreateShortUrlCommandValidatorTests
{
    private readonly CreateShortUrlCommandValidator _sut;
    
    public CreateShortUrlCommandValidatorTests()
    {
        _sut = new CreateShortUrlCommandValidator();
    }
    
    [Theory]
    [InlineData("https://www.google.com", true)]
    [InlineData("www.googke.com", true)]
    [InlineData("noturl", false, "Url is not valid.")]
    [InlineData("", false, "Url is required.")]
    public void TestValidator(string url, bool isValid, string? validationError = null)
    {
        var command = new CreateShortUrlCommand { Url = url }; 
        var validationResult = _sut.TestValidate(command);

        if (isValid)
        {
            validationResult.ShouldNotHaveValidationErrorFor(command => command.Url);
        }
        else
        {
            validationResult.ShouldHaveValidationErrorFor(command => command.Url).WithErrorMessage(validationError);
        }
    }
}
