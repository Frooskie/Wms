using FluentValidation;
using Wms.API.DTOs.Supply;
using Wms.Core.Constants;

namespace Wms.API.Validators.Supply;

public class CreateSupplyRequestValidator : AbstractValidator<CreateSupplyRequestRequest>
{
    public CreateSupplyRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Lines)
            .NotNull().WithMessage(ErrorMessages.Validation.LinesRequired)
            .Must(list => list.Count > 0).WithMessage(ErrorMessages.Validation.AtLeastOneLineRequired);

        RuleForEach(x => x.Lines)
            .SetValidator(new SupplyRequestLineValidator());
    }
}