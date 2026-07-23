namespace Wms.Core.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object id) 
        : base($"Entity '{entityName}' with id '{id}' was not found.") { }
}