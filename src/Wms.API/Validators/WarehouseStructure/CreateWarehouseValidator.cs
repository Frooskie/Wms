using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;

namespace Wms.API.Validators.WarehouseStructure;

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);
    }
}