using FluentValidation;
using Wms.API.DTOs.Receipts;
using Wms.Core.Constants;

namespace Wms.API.Validators.Receipts;

public class CreateReceiptValidator : AbstractValidator<CreateReceiptRequest>
{
    public CreateReceiptValidator()
    {
        RuleFor(x => x.Supplier)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Lines)
            .NotNull().WithMessage(ErrorMessages.Validation.LinesRequired)
            .Must(list => list.Count > 0).WithMessage(ErrorMessages.Validation.AtLeastOneLineRequired);

        RuleForEach(x => x.Lines)
            .SetValidator(new ReceiptLineValidator());
    }
}