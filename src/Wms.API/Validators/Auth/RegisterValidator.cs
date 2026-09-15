using FluentValidation;
using Wms.API.DTOs.Auth;
using Wms.Core.Constants;

namespace Wms.API.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.Validation.EmailRequired)
            .EmailAddress().WithMessage(ErrorMessages.Validation.InvalidEmailFormat);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PasswordRequired)
            .MinimumLength(6).WithMessage(ErrorMessages.Validation.PasswordMinLength(6));

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorMessages.Validation.FullNameRequired)
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(ErrorMessages.Validation.RoleRequired)
            .Must(r => !string.IsNullOrEmpty(r) && Roles.All.Contains(r))
            .WithMessage(ErrorMessages.Auth.RoleMustBeOneOfFormat(string.Join(", ", Roles.All)));
    }
}