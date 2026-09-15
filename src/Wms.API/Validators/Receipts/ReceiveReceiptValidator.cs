using FluentValidation;
using Wms.API.DTOs.Receipts;
using Wms.Core.Constants;

namespace Wms.API.Validators.Receipts;

public class ReceiveReceiptValidator : AbstractValidator<ReceiveReceiptRequest>
{
    public ReceiveReceiptValidator()
    {
        RuleFor(x => x.Lines)
            .NotNull().WithMessage(ErrorMessages.Validation.LinesRequired)
            .Must(list => list.Count > 0).WithMessage(ErrorMessages.Validation.AtLeastOneLineRequired);

        RuleForEach(x => x.Lines)
            .SetValidator(new ReceiveReceiptLineValidator());
    }
}