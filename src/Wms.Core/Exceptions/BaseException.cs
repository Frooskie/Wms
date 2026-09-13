namespace Wms.Core.Exceptions;

public abstract class BaseException : Exception
{
    /// <summary>
    /// Машиночитаемый код ошибки. Используется клиентом (фронтендом)
    /// для распознавания типа ошибки независимо от текста сообщения.
    /// </summary>
    public string Code { get; }

    protected BaseException(string message, string code = "INTERNAL_ERROR")
        : base(message)
    {
        Code = code;
    }

    protected BaseException(string message, string code, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}