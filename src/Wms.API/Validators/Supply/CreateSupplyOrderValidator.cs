using FluentValidation;
using Wms.API.DTOs.Supply;

namespace Wms.API.Validators.Supply;

public class CreateSupplyOrderValidator : AbstractValidator<CreateSupplyOrderRequest>
{
    public CreateSupplyOrderValidator()
    {
        RuleFor(x => x.Lines)
            .NotNull().WithMessage("Lines are required.")
            .Must(list => list.Count > 0).WithMessage("At least one line is required.");

        RuleForEach(x => x.Lines)
            .SetValidator(new SupplyOrderLineValidator());
    }
}