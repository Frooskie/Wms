using FluentValidation;
using Wms.Core.Exceptions;

namespace Wms.API.Extensions;

public static class ValidatorExtensions
{
    public static async Task ValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();
            throw new ModelValidationException(string.Join("; ", errors));
        }
    }
}