using FluentValidation;
using Shortify.BusinessLogic.DTOs;

namespace Shortify.BusinessLogic.Validation;

public class GetUrlDtoValidator : AbstractValidator<GetUrlDto>
{
    public GetUrlDtoValidator()
    {
        RuleFor(x => x.ShortUrl)
            .NotEmpty()
            .Matches(@"^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$")
            .WithMessage("Invalid URL provided");
    }
}