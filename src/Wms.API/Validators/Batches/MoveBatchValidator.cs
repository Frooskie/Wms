using FluentValidation;
using Wms.API.DTOs.Batches;
using Wms.Core.Constants;

namespace Wms.API.Validators.Batches;

public class MoveBatchValidator : AbstractValidator<MoveBatchRequest>
{
    public MoveBatchValidator()
    {
        RuleFor(x => x.CellId)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.Batch.CellIdPositive);
    }
}