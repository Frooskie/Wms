using FluentValidation;
using Wms.API.DTOs.Receipts;

namespace Wms.API.Validators.Receipts;

public class ReceiptLineValidator : AbstractValidator<ReceiptLineRequest>
{
    public ReceiptLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.ExpectedQuantity)
            .GreaterThan(0);
    }
}