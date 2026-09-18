using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class UnauthorizedException(string message = ErrorMessages.Common.Unauthorized)
    : BaseException(message, ErrorCodes.Unauthorized);