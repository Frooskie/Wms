using FluentValidation;
using Wms.API.DTOs.Supply;

namespace Wms.API.Validators.Supply;

public class SupplyRequestLineValidator : AbstractValidator<SupplyRequestLineRequest>
{
    public SupplyRequestLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.RequestedQuantity)
            .GreaterThan(0);
    }
}