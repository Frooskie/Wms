using FluentValidation;
using Wms.API.DTOs.Batches;
using Wms.Core.Constants;

namespace Wms.API.Validators.Batches;

public class CreateBatchValidator : AbstractValidator<CreateBatchRequest>
{
    public CreateBatchValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.Batch.PurchasePriceNegative);

        RuleFor(x => x.ProductionDate)
            .Must(date => date <= DateTime.UtcNow)
            .WithMessage(ErrorMessages.Batch.ProductionDateInFuture);

        RuleFor(x => x.ExpiryDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage(ErrorMessages.Batch.ExpiryDateMustBeFuture);

        RuleFor(x => x.CellId)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.ExpiryDate > x.ProductionDate)
            .WithMessage(ErrorMessages.Batch.ExpiryDateAfterProduction);
    }
}