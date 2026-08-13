using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;

namespace Wms.API.Validators.WarehouseStructure;

public class CreateShelfValidator : AbstractValidator<CreateShelfRequest>
{
    public CreateShelfValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0);
        
        RuleFor(x => x.RackId)
            .GreaterThan(0);
    }
}