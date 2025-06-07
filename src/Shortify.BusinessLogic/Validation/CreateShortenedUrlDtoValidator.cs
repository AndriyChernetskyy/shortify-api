using FluentValidation;
using Shortify.BusinessLogic.DTOs;

namespace Shortify.BusinessLogic.Validation;

public class CreateShortenedUrlDtoValidator : AbstractValidator<CreateShortenedUrlDto>
{
    public CreateShortenedUrlDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .Matches(@"^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$")
            .WithMessage("Invalid URL provided");
    }
}