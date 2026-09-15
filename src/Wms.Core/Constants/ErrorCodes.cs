namespace Wms.Core.Constants;

public static class ErrorCodes
{
    public const string NotFound = "NOT_FOUND";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string CellOccupied = "CELL_OCCUPIED";
    public const string TargetCellOccupied = "TARGET_CELL_OCCUPIED";
    public const string NotEnoughStock = "NOT_ENOUGH_STOCK";
    public const string OrderNotDraft = "ORDER_NOT_DRAFT";
    public const string OrderNotConfirmed = "ORDER_NOT_CONFIRMED";
    public const string OrderHasNoReservations = "ORDER_HAS_NO_RESERVATIONS";
    public const string ReceiptAlreadyProcessed = "RECEIPT_ALREADY_PROCESSED";
    public const string RequestNotDraft = "REQUEST_NOT_DRAFT";
    public const string RequestNotSubmitted = "REQUEST_NOT_SUBMITTED";
    public const string RequestNotRejectable = "REQUEST_NOT_REJECTABLE";
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string InvalidStatus = "INVALID_STATUS";
    public const string InvalidTransactionType = "INVALID_TRANSACTION_TYPE";
}