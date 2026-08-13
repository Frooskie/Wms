using FluentValidation;
using Wms.API.DTOs.Batches;

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
            .GreaterThanOrEqualTo(0).WithMessage("PurchasePrice cannot be negative.");

        RuleFor(x => x.ProductionDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Production date cannot be in the future.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future.");

        RuleFor(x => x.CellId)
            .GreaterThan(0);
        
        RuleFor(x => x)
            .Must(x => x.ExpiryDate > x.ProductionDate)
            .WithMessage("Expiry date must be after production date.");
    }
}