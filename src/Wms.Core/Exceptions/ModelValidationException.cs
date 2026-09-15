using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class ModelValidationException(string message, IEnumerable<ValidationError>? errors = null)
    : BaseException(message, ErrorCodes.ValidationFailed)
{
    public IEnumerable<ValidationError> Errors { get; } = errors ?? [];
}