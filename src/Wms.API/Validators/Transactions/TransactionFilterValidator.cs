using FluentValidation;
using Wms.API.DTOs.Transactions;
using Wms.Core.Enums;

namespace Wms.API.Validators.Transactions;

public class TransactionFilterValidator : AbstractValidator<TransactionFilterDto>
{
    public TransactionFilterValidator()
    {
        RuleFor(x => x.BatchId)
            .GreaterThan(0)
            .When(x => x.BatchId.HasValue)
            .WithMessage("BatchId must be positive if specified.");
        
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .When(x => x.ProductId.HasValue)
            .WithMessage("ProductId must be positive if specified.");
        
        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.UserId))
            .WithMessage("UserId must not be empty if specified.");
        
        RuleFor(x => x.TransactionType)
            .Must(value => string.IsNullOrEmpty(value) || Enum.IsDefined(typeof(TransactionType), value))
            .WithMessage("Invalid TransactionType. Allowed values: In, Out, Move, WriteOff.");
        
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("FromDate must be less than or equal to ToDate.");
    }
}