using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message, ErrorCodes.NotFound)
    {
    }

    public NotFoundException(string entityName, object id)
        : base($"Сущность '{entityName}' с идентификатором '{id}' не найдена.", ErrorCodes.NotFound)
    {
    }
}