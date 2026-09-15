using FluentValidation;
using Wms.API.DTOs.Products;
using Wms.Core.Constants;

namespace Wms.API.Validators.Products;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Unit)
            .NotEmpty().MaximumLength(20);

        RuleFor(x => x.MinStockThreshold)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.Product.MinStockThresholdNonNegative);
    }
}