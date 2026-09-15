using FluentValidation;
using Wms.API.DTOs.Transactions;
using Wms.Core.Constants;
using Wms.Core.Enums;

namespace Wms.API.Validators.Transactions;

public class TransactionFilterValidator : AbstractValidator<TransactionFilterDto>
{
    public TransactionFilterValidator()
    {
        RuleFor(x => x.BatchId)
            .GreaterThan(0)
            .When(x => x.BatchId.HasValue)
            .WithMessage(ErrorMessages.Validation.MustBePositiveIfSpecified);

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .When(x => x.ProductId.HasValue)
            .WithMessage(ErrorMessages.Validation.MustBePositiveIfSpecified);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.UserId))
            .WithMessage(ErrorMessages.Validation.MustNotBeEmptyIfSpecified);

        RuleFor(x => x.TransactionType)
            .Must(value => string.IsNullOrEmpty(value)
                           || Enum.TryParse<TransactionType>(value, true, out _))
            .WithMessage(ErrorMessages.Transactions.InvalidTransactionType);

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage(ErrorMessages.Validation.DateRangeInvalid);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ErrorMessages.Validation.PageNumberMin);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(ErrorMessages.Validation.PageSizeRange);
    }
}