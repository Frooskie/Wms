namespace Wms.Core.Exceptions;

public class UnauthorizedException(string message = "Authentication failed.")
    : BaseException(message, "UNAUTHORIZED");