namespace Wms.Core.Exceptions;

public class ForbiddenAccessException(string message = "You do not have permission to perform this action.")
    : BaseException(message, "FORBIDDEN");