using FluentValidation;
using Wms.API.DTOs.Receipts;

namespace Wms.API.Validators.Receipts;

public class CreateReceiptValidator : AbstractValidator<CreateReceiptRequest>
{
    public CreateReceiptValidator()
    {
        RuleFor(x => x.Supplier)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Lines)
            .NotNull().WithMessage("Lines must be provided.")
            .Must(list => list.Count > 0).WithMessage("At least one line is required.");

        RuleForEach(x => x.Lines)
            .SetValidator(new ReceiptLineValidator());
    }
}