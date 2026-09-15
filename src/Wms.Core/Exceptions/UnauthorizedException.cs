using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class UnauthorizedException(string message = "Authentication failed.")
    : BaseException(message, ErrorCodes.Unauthorized);