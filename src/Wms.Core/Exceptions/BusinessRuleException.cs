namespace Wms.Core.Exceptions;

public class BusinessRuleException(string message, string code = "BUSINESS_RULE_VIOLATION")
    : BaseException(message, code);