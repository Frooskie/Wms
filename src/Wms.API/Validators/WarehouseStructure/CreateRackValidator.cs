using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;

namespace Wms.API.Validators.WarehouseStructure;

public class CreateRackValidator : AbstractValidator<CreateRackRequest>
{
    public CreateRackValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().MaximumLength(100);
        
        RuleFor(x => x.ZoneId)
            .GreaterThan(0);
    }
}