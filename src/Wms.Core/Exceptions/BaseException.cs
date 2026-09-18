using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public abstract class BaseException : Exception
{
    /// <summary>
    /// Машиночитаемый код ошибки. Используется клиентом (фронтендом)
    /// для распознавания типа ошибки независимо от текста сообщения.
    /// </summary>
    public ErrorCodes Code { get; }

    protected BaseException(string message, ErrorCodes code = ErrorCodes.InternalError)
        : base(message)
    {
        Code = code;
    }

    protected BaseException(string message, ErrorCodes code, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}