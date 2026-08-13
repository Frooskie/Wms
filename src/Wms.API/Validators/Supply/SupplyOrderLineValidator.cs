using FluentValidation;
using Wms.API.DTOs.Supply;

namespace Wms.API.Validators.Supply;

public class SupplyOrderLineValidator : AbstractValidator<SupplyOrderLineRequest>
{
    public SupplyOrderLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}