namespace Wms.API.Validators;

/// <summary>
/// Словарь переводов имён свойств DTO для отображения в сообщениях валидации.
/// </summary>
public static class PropertyDisplayNames
{
    private static readonly Dictionary<string, string> Translations = new()
    {
        // Auth
        ["Email"] = "Email",
        ["Password"] = "Пароль",
        ["FullName"] = "Полное имя",
        ["Role"] = "Роль",
        ["Token"] = "Токен",

        // Products
        ["Name"] = "Название",
        ["Manufacturer"] = "Производитель",
        ["Supplier"] = "Поставщик",
        ["Category"] = "Категория",
        ["Unit"] = "Единица измерения",
        ["MinStockThreshold"] = "Минимальный порог остатка",

        // Warehouse structure
        ["Address"] = "Адрес",
        ["ContactPhone"] = "Контактный телефон",
        ["Type"] = "Тип",
        ["WarehouseId"] = "Идентификатор склада",
        ["ZoneId"] = "Идентификатор зоны",
        ["RackId"] = "Идентификатор стеллажа",
        ["ShelfId"] = "Идентификатор полки",
        ["Code"] = "Код",
        ["Number"] = "Номер",

        // Batches
        ["ProductId"] = "Идентификатор товара",
        ["BatchId"] = "Идентификатор партии",
        ["CellId"] = "Идентификатор ячейки",
        ["Quantity"] = "Количество",
        ["PurchasePrice"] = "Закупочная цена",
        ["ProductionDate"] = "Дата производства",
        ["ExpiryDate"] = "Срок годности",
        ["ExpiryFrom"] = "Начальная дата срока годности",
        ["ExpiryTo"] = "Конечная дата срока годности",

        // Receipts
        ["Lines"] = "Позиции",
        ["ExpectedQuantity"] = "Ожидаемое количество",
        ["ActualQuantity"] = "Фактическое количество",

        // Supply
        ["StoreName"] = "Название магазина",
        ["SupplyRequestId"] = "Идентификатор заявки",
        ["RequestedQuantity"] = "Запрошенное количество",

        // Transactions
        ["TransactionType"] = "Тип транзакции",
        ["UserId"] = "Идентификатор пользователя",
        ["FromDate"] = "Дата от",
        ["ToDate"] = "Дата до",

        // Pagination
        ["PageNumber"] = "Номер страницы",
        ["PageSize"] = "Размер страницы",
    };

    /// <summary>
    /// Возвращает русское отображаемое имя для свойства.
    /// Если перевода нет — возвращает исходное имя.
    /// </summary>
    public static string Resolve(string propertyName)
    {
        return Translations.GetValueOrDefault(propertyName, propertyName);
    }
}