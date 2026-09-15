using FluentValidation;
using Wms.API.DTOs.Batches;
using Wms.Core.Constants;

namespace Wms.API.Validators.Batches;

public class FilterBatchValidator : AbstractValidator<BatchFilterDto>
{
    public FilterBatchValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ErrorMessages.Validation.PageNumberMin);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(ErrorMessages.Validation.PageSizeRange);

        RuleFor(x => x.ExpiryFrom)
            .LessThanOrEqualTo(x => x.ExpiryTo)
            .When(x => x.ExpiryFrom.HasValue && x.ExpiryTo.HasValue)
            .WithMessage(ErrorMessages.Batch.ExpiryFromAfterExpiryTo);
    }
}