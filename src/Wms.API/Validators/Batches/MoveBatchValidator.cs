using FluentValidation;
using Wms.API.DTOs.Batches;

namespace Wms.API.Validators.Batches;

public class MoveBatchValidator : AbstractValidator<MoveBatchRequest>
{
    public MoveBatchValidator()
    {
        RuleFor(x => x.CellId)
            .GreaterThan(0).WithMessage("CellId must be a positive integer.");
    }
}