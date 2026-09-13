namespace Wms.Core.Exceptions;

public class ModelValidationException(string message, IEnumerable<ValidationError>? errors = null)
    : BaseException(message, "VALIDATION_FAILED")
{
    public IEnumerable<ValidationError> Errors { get; } = errors ?? [];
}