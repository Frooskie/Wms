using FluentValidation;
using Wms.API.DTOs.Products;

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
            .GreaterThanOrEqualTo(0).WithMessage("MinStockThreshold must be non-negative.");
    }
}