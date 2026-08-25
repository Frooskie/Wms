using FluentValidation;
using Wms.API.DTOs.Batches;

namespace Wms.API.Validators.Batches;

public class FilterBatchValidator : AbstractValidator<BatchFilterDto>
{
    public FilterBatchValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("Page number must be >= 1.");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        RuleFor(x => x.ExpiryFrom).LessThanOrEqualTo(x => x.ExpiryTo)
            .When(x => x.ExpiryFrom.HasValue && x.ExpiryTo.HasValue)
            .WithMessage("ExpiryFrom must be before or equal to ExpiryTo.");
    }
}