using FluentValidation;
using Wms.API.DTOs.Auth;
using Wms.Core.Constants;

namespace Wms.API.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.Validation.EmailRequired)
            .EmailAddress().WithMessage(ErrorMessages.Validation.InvalidEmailFormat);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PasswordRequired)
            .MinimumLength(6).WithMessage(ErrorMessages.Validation.PasswordMinLength(6));
    }
}