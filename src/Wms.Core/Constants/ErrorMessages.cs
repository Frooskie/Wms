namespace Wms.Core.Constants;

/// <summary>
/// Единый реестр всех сообщений об ошибках, которые видит пользователь.
/// Тексты на русском языке. При необходимости легко заменить на .resx-файлы
/// для мультиязычности.
/// </summary>
public static class ErrorMessages
{
    /// <summary>Общие сообщения, не привязанные к конкретной сущности.</summary>
    public static class Common
    {
        public const string InternalError = "Произошла внутренняя ошибка. Попробуйте позже.";
        public const string Unauthorized = "Требуется авторизация.";
        public const string Forbidden = "Недостаточно прав для выполнения операции.";
        public const string InvalidRequest = "Некорректный запрос.";
        public const string UserNotAuthenticated = "Пользователь не аутентифицирован.";
        public const string CannotDeleteReferenced = "Нельзя удалить объект: на него ссылаются связанные записи.";
    }

    /// <summary>Сообщения валидации входных данных (FluentValidation).</summary>
    public static class Validation
    {
        public const string Required = "Поле обязательно для заполнения.";
        public const string MustBePositive = "Значение должно быть больше нуля.";
        public const string MustBeNonNegative = "Значение не может быть отрицательным.";
        public const string InvalidEmail = "Некорректный email.";
        public const string InvalidFormat = "Некорректный формат значения.";
        public const string TooLong = "Значение слишком длинное.";
        public const string TooShort = "Значение слишком короткое.";
        public const string LinesRequired = "Позиции обязательны для заполнения.";
        public const string AtLeastOneLineRequired = "Должна быть указана хотя бы одна позиция.";
        public const string MustBePositiveIfSpecified = "Значение должно быть положительным, если указано.";
        public const string MustNotBeEmptyIfSpecified = "Значение не должно быть пустым, если указано.";
        public const string InvalidEnumValue = "Недопустимое значение.";
        public const string DateRangeInvalid = "Дата начала не может быть позже даты окончания.";
        public const string PageSizeRange = "Размер страницы должен быть от 1 до 100.";
        public const string PageNumberMin = "Номер страницы должен быть не меньше 1.";
        public const string EmailRequired = "Email обязателен для заполнения.";
        public const string InvalidEmailFormat = "Некорректный формат email.";
        public const string PasswordRequired = "Пароль обязателен для заполнения.";
        public const string FullNameRequired = "Полное имя обязательно для заполнения.";
        public const string RoleRequired = "Роль обязательна для заполнения.";

        public static string PasswordMinLength(int min) =>
            $"Пароль должен содержать минимум {min} символов.";

        public static string MaxLength(int max) =>
            $"Длина не должна превышать {max} символов.";

        public static string MinLength(int min) =>
            $"Длина должна быть не менее {min} символов.";

        public static string GreaterThan(decimal value) =>
            $"Значение должно быть больше {value}.";

        public static string GreaterThanOrEqual(decimal value) =>
            $"Значение должно быть не меньше {value}.";
    }

    /// <summary>Сообщения, связанные с аутентификацией и регистрацией.</summary>
    public static class Auth
    {
        public const string InvalidCredentials = "Неверный email или пароль.";
        public const string UserCreationFailed = "Не удалось создать пользователя.";
        public const string RoleNotFoundFormat = "Роль '{0}' не существует.";
        public const string PasswordsDoNotMatch = "Пароли не совпадают.";
        
        public static string RoleMustBeOneOfFormat(string allowed) =>
            $"Роль должна быть одной из: {allowed}.";
    }

    /// <summary>Сообщения, связанные с товарами.</summary>
    public static class Product
    {
        public const string NotFound = "Товар не найден.";
        public const string MinStockThresholdNonNegative = "Минимальный порог остатка не может быть отрицательным.";

        public static string NotFoundFormat(int id) =>
            $"Товар с идентификатором {id} не найден.";
    }

    /// <summary>Сообщения, связанные с партиями.</summary>
    public static class Batch
    {
        public const string NotFound = "Партия не найдена.";
        public const string CellNotFound = "Ячейка не найдена.";
        public const string CellOccupied = "Ячейка уже занята другой партией.";
        public const string TargetCellNotFound = "Целевая ячейка не найдена.";
        public const string TargetCellOccupied = "Целевая ячейка уже занята.";
        public const string PurchasePriceNegative = "Закупочная цена не может быть отрицательной.";
        public const string ProductionDateInFuture = "Дата производства не может быть в будущем.";
        public const string ExpiryDateMustBeFuture = "Срок годности должен быть в будущем.";
        public const string ExpiryDateAfterProduction = "Срок годности должен быть позже даты производства.";
        public const string CellIdPositive = "Идентификатор ячейки должен быть положительным числом.";
        public const string ExpiryFromAfterExpiryTo = "Дата начала срока годности должна быть не позже даты окончания.";

        public static string NotFoundFormat(int id) =>
            $"Партия с идентификатором {id} не найдена.";
    }

    /// <summary>Сообщения, связанные с ячейками.</summary>
    public static class Cell
    {
        public const string NotFound = "Ячейка не найдена.";
        public const string AlreadyOccupied = "Ячейка уже занята.";

        public static string AlreadyOccupiedFormat(string code) =>
            $"Ячейка '{code}' уже занята.";

        public static string NotFoundFormat(int id) =>
            $"Ячейка с идентификатором {id} не найдена.";
    }

    /// <summary>Сообщения, связанные с приёмкой товаров.</summary>
    public static class Receipt
    {
        public const string NotFound = "Документ приёмки не найден.";
        public const string AlreadyProcessed = "Документ приёмки уже обработан.";
        public const string ExpiryDateMustBeFuture = "Срок годности должен быть в будущем.";

        public static string NotFoundFormat(int id) =>
            $"Документ приёмки с идентификатором {id} не найден.";

        public static string AlreadyProcessedFormat(string status) =>
            $"Документ приёмки уже находится в статусе '{status}'.";

        public static string ProductNotFound(int productId) =>
            $"Товар с идентификатором {productId} не найден.";

        public static string CellNotFound(int cellId) =>
            $"Ячейка с идентификатором {cellId} не найдена.";

        public static string CellOccupied(string cellCode) =>
            $"Ячейка '{cellCode}' уже занята.";

        // Расхождения при приёмке — в подробном виде
        public const string DiscrepancyMissing = "Товар {0}: ожидалось {1}, но фактически отсутствует.";
        public const string DiscrepancyQuantity = "Товар {0}: ожидалось {1}, фактически {2}.";
    }

    /// <summary>Сообщения, связанные с заявками магазина.</summary>
    public static class SupplyRequest
    {
        public const string NotFound = "Заявка не найдена.";
        public const string OnlyCreatorCanSubmit = "Только создатель может отправить заявку на рассмотрение.";
        public const string OnlyDraftCanBeSubmitted = "Только черновик можно отправить на рассмотрение.";
        public const string OnlySubmittedCanBeApproved = "Только отправленную заявку можно одобрить.";

        public const string OnlyDraftOrSubmittedCanBeRejected =
            "Только черновик или отправленную заявку можно отклонить.";

        public static string NotFoundFormat(int id) =>
            $"Заявка с идентификатором {id} не найдена.";
    }

    /// <summary>Сообщения, связанные с заказами на отгрузку.</summary>
    public static class SupplyOrder
    {
        public const string NotFound = "Заказ на отгрузку не найден.";
        public const string NoReservations = "У заказа нет резервов. Отгрузка невозможна.";
        public const string NotEnoughStock = "Недостаточно остатков на складе.";

        public static string NotFoundFormat(int id) =>
            $"Заказ на отгрузку с идентификатором {id} не найден.";

        public static string OnlyDraftCanBeConfirmed(string status) =>
            $"Нельзя подтвердить заказ в статусе '{status}'. Только черновик можно подтвердить.";

        public static string OnlyConfirmedCanBeShipped(string status) =>
            $"Нельзя отгрузить заказ в статусе '{status}'. Только подтверждённый заказ можно отгрузить.";

        public static string NotEnoughStockFormat(int productId, int missing) =>
            $"Недостаточно остатков для товара ID {productId}. Не хватает {missing} ед.";
    }
    
    /// <summary>Сообщения, связанные с транзакциями аудита.</summary>
    public static class Transactions
    {
        public const string InvalidTransactionType = "Недопустимое значение типа транзакции.";

        public static string InvalidTransactionTypeFormat(string value, string allowed) =>
            $"Недопустимое значение типа транзакции '{value}'. Допустимые значения: {allowed}.";
    }

    /// <summary>Сообщения, связанные с уведомлениями.</summary>
    public static class Notification
    {
        public const string NotFound = "Уведомление не найдено.";
        public const string AccessDenied = "У вас нет доступа к этому уведомлению.";

        public static string NotFoundFormat(int id) =>
            $"Уведомление с идентификатором {id} не найдено.";
    }
    

    /// <summary>Сообщения, связанные со складской иерархией.</summary>
    public static class WarehouseStructure
    {
        public const string WarehouseNotFound = "Склад не найден.";
        public const string ZoneNotFound = "Зона не найдена.";
        public const string RackNotFound = "Стеллаж не найден.";
        public const string ShelfNotFound = "Полка не найдена.";
        public const string CellNotFound = "Ячейка не найдена.";

        public const string CannotDeleteRackWithBatches = "Нельзя удалить стеллаж, в котором есть партии.";
        public const string CannotDeleteShelfWithBatches = "Нельзя удалить полку, в которой есть партии.";
        public const string CannotDeleteCellWithBatches = "Нельзя удалить ячейку, в которой есть партия.";
        
        public const string InvalidZoneType = "Недопустимый тип зоны.";
        public const string InvalidWarehouseId = "Некорректный идентификатор склада.";
        public const string InvalidZoneId = "Некорректный идентификатор зоны.";
        public const string InvalidRackId = "Некорректный идентификатор стеллажа.";
        public const string InvalidShelfId = "Некорректный идентификатор полки.";
        public const string InvalidCodeFormat = "Код должен содержать только буквы и цифры.";
        public const string InvalidShelfNumber = "Номер полки должен быть положительным числом.";
    }
    
    /// <summary>Сообщения об успешных операциях.</summary>
    public static class Success
    {
        public const string UserRegistered = "Пользователь успешно зарегистрирован.";
    }
}