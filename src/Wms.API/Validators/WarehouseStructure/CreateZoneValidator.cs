using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Constants;

namespace Wms.API.Validators.WarehouseStructure;

public class CreateZoneValidator : AbstractValidator<CreateZoneRequest>
{
    public CreateZoneValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage(ErrorMessages.WarehouseStructure.InvalidZoneType)
            .IsInEnum()
            .WithMessage(ErrorMessages.WarehouseStructure.InvalidZoneType);
    }
}