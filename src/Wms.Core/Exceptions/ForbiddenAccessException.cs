using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class ForbiddenAccessException(string message = ErrorMessages.Common.Forbidden)
    : BaseException(message, ErrorCodes.Forbidden);