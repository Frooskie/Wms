using FluentValidation;
using Wms.API.DTOs.Receipts;
using Wms.Core.Constants;

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
            .Must(date => date > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage(ErrorMessages.Receipt.ExpiryDateMustBeFuture);

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0);
    }
}