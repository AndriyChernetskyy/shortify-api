using FluentValidation;

namespace Shortify.BusinessLogic.Validation;

public class GetUrlValidator : AbstractValidator<string>
{
    public GetUrlValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .Length(4);
    }
}