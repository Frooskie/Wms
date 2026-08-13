using FluentValidation;
using Wms.API.DTOs.Supply;

namespace Wms.API.Validators.Supply;

public class CreateSupplyRequestValidator : AbstractValidator<CreateSupplyRequestRequest>
{
    public CreateSupplyRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Lines)
            .NotNull().WithMessage("Lines are required.")
            .Must(list => list.Count > 0).WithMessage("At least one line is required.");

        RuleForEach(x => x.Lines)
            .SetValidator(new SupplyRequestLineValidator());
    }
}