using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;

namespace Wms.API.Validators.WarehouseStructure;

public class UpdateShelfValidator : AbstractValidator<UpdateShelfRequest>
{
    public UpdateShelfValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0);
        
        RuleFor(x => x.RackId)
            .GreaterThan(0);
    }
}