using FluentValidation;
using Wms.API.DTOs.Receipts;

namespace Wms.API.Validators.Receipts;

public class ReceiveReceiptLineValidator : AbstractValidator<ReceiveReceiptLineRequest>
{
    public ReceiveReceiptLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.ActualQuantity)
            .GreaterThan(0);

        RuleFor(x => x.CellId)
            .GreaterThan(0);

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0);
    }
}