using FluentValidation;
using Wms.API.DTOs.WarehouseStructure;

namespace Wms.API.Validators.WarehouseStructure;

public class UpdateCellValidator : AbstractValidator<UpdateCellRequest>
{
    public UpdateCellValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().MaximumLength(100);
        
        RuleFor(x => x.ShelfId)
            .GreaterThan(0);
    }
}