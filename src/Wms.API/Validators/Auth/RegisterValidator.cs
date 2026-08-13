using FluentValidation;
using Wms.API.DTOs.Auth;

namespace Wms.API.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(6);

        RuleFor(x => x.FullName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => new[] { "Chief", "Manager", "Worker", "StoreDirector" }.Contains(r))
            .WithMessage("Role must be one of: Chief, Manager, Worker, StoreDirector.");
    }
}