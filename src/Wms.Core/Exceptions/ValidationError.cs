namespace Wms.Core.Exceptions;

public record ValidationError(string PropertyName, string ErrorMessage)
{
    
};