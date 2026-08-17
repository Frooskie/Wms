namespace Wms.Core.Exceptions;

public class ModelValidationException(string message, IEnumerable<ValidationError>? errors = null)
    : BaseException(message)
{
    public IEnumerable<ValidationError> Errors { get; } = errors ?? [];
}