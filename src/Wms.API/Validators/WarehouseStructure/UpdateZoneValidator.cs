using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Enums;

namespace Wms.API.Validators.WarehouseStructure;

public class UpdateZoneValidator : AbstractValidator<UpdateZoneRequest>
{
    public  UpdateZoneValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid zone type.");
    }
}