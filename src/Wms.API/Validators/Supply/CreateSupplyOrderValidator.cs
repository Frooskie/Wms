using FluentValidation;
using Wms.API.DTOs.Supply;
using Wms.Core.Constants;

namespace Wms.API.Validators.Supply;

public class CreateSupplyOrderValidator : AbstractValidator<CreateSupplyOrderRequest>
{
    public CreateSupplyOrderValidator()
    {
        RuleFor(x => x.Lines)
            .NotNull().WithMessage(ErrorMessages.Validation.LinesRequired)
            .Must(list => list.Count > 0).WithMessage(ErrorMessages.Validation.AtLeastOneLineRequired);

        RuleForEach(x => x.Lines)
            .SetValidator(new SupplyOrderLineValidator());
    }
}