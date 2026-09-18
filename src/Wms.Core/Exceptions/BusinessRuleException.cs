using Wms.Core.Constants;

namespace Wms.Core.Exceptions;

public class BusinessRuleException(string message, ErrorCodes code = ErrorCodes.BusinessRuleViolation)
    : BaseException(message, code);