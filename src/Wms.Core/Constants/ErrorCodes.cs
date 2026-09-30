using System.Runtime.Serialization;

namespace Wms.Core.Constants;

/// <summary>
/// Машиночитаемые коды ошибок API.
/// Используются в <see cref="Exceptions.BaseException.Code"/>,
/// в ответах ProblemDetails и на фронтенде (генерируется через NSwag).
/// Значения сериализуются в строку через <see cref="EnumMemberAttribute"/>.
/// </summary>
public enum ErrorCodes
{
    // ===== Общие =====

    /// <summary>Непредвиденная ошибка сервера.</summary>
    [EnumMember(Value = "INTERNAL_ERROR")]
    InternalError,

    /// <summary>Запрошенный ресурс не найден.</summary>
    [EnumMember(Value = "NOT_FOUND")]
    NotFound,

    /// <summary>Ошибка валидации входных данных.</summary>
    [EnumMember(Value = "VALIDATION_FAILED")]
    ValidationFailed,

    /// <summary>Неверные учётные данные или отсутствует токен.</summary>
    [EnumMember(Value = "UNAUTHORIZED")]
    Unauthorized,

    /// <summary>Недостаточно прав для операции.</summary>
    [EnumMember(Value = "FORBIDDEN")]
    Forbidden,
    
    /// <summary>Нельзя удалить объект, на который ссылаются связанные записи.</summary>
    [EnumMember(Value = "CANNOT_DELETE_REFERENCED")]
    CannotDeleteReferenced,

    // ===== Партии и ячейки =====

    /// <summary>Ячейка уже занята другой партией.</summary>
    [EnumMember(Value = "CELL_OCCUPIED")]
    CellOccupied,

    /// <summary>Целевая ячейка занята при перемещении.</summary>
    [EnumMember(Value = "TARGET_CELL_OCCUPIED")]
    TargetCellOccupied,

    // ===== Приёмка =====

    /// <summary>Документ приёмки уже обработан (подтверждён или отклонён).</summary>
    [EnumMember(Value = "RECEIPT_ALREADY_PROCESSED")]
    ReceiptAlreadyProcessed,

    // ===== Заказы на отгрузку =====

    /// <summary>Подтверждение заказа, не находящегося в статусе Draft.</summary>
    [EnumMember(Value = "ORDER_NOT_DRAFT")]
    OrderNotDraft,

    /// <summary>Отгрузка заказа, не находящегося в статусе Confirmed.</summary>
    [EnumMember(Value = "ORDER_NOT_CONFIRMED")]
    OrderNotConfirmed,

    /// <summary>Отгрузка заказа без резервов.</summary>
    [EnumMember(Value = "ORDER_HAS_NO_RESERVATIONS")]
    OrderHasNoReservations,

    /// <summary>Недостаточно доступных остатков для подтверждения заказа.</summary>
    [EnumMember(Value = "NOT_ENOUGH_STOCK")]
    NotEnoughStock,

    // ===== Заявки магазина =====

    /// <summary>Отправка заявки, не находящейся в статусе Draft.</summary>
    [EnumMember(Value = "REQUEST_NOT_DRAFT")]
    RequestNotDraft,

    /// <summary>Одобрение заявки, не находящейся в статусе Submitted.</summary>
    [EnumMember(Value = "REQUEST_NOT_SUBMITTED")]
    RequestNotSubmitted,

    /// <summary>Отклонение заявки в неподходящем статусе.</summary>
    [EnumMember(Value = "REQUEST_NOT_REJECTABLE")]
    RequestNotRejectable,

    // ===== Роли =====

    /// <summary>Указана несуществующая роль.</summary>
    [EnumMember(Value = "ROLE_NOT_FOUND")]
    RoleNotFound,

    // ===== Фильтры и параметры =====

    /// <summary>Недопустимое значение статуса в query-параметре.</summary>
    [EnumMember(Value = "INVALID_STATUS")]
    InvalidStatus,

    /// <summary>Недопустимое значение типа транзакции.</summary>
    [EnumMember(Value = "INVALID_TRANSACTION_TYPE")]
    InvalidTransactionType,

    // ===== Бизнес-правила (общий код по умолчанию) =====

    /// <summary>
    /// Общий код для BusinessRuleException, если конкретный код не указан.
    /// Использовать по возможности конкретные коды вместо этого.
    /// </summary>
    [EnumMember(Value = "BUSINESS_RULE_VIOLATION")]
    BusinessRuleViolation,
}