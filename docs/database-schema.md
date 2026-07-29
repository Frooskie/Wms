# Схема базы данных WMS (полная версия)

## Обозначения

- `PK` — первичный ключ
- `FK` — внешний ключ
- `UQ` — уникальное ограничение (индекс)
- `NOT NULL` — поле обязательное
- `text` — тип данных в PostgreSQL (varchar или text)

---

## Таблицы Identity (ASP.NET Core Identity)

Используются стандартные таблицы, предоставляемые библиотекой `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.

| Таблица | Назначение |
|---------|------------|
| `AspNetUsers` | Пользователи системы (расширен полем `FullName`). |
| `AspNetRoles` | Роли (`Chief`, `Manager`, `Worker`, `StoreDirector`). |
| `AspNetUserRoles` | Связь пользователей с ролями. |
| `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens` | Стандартные таблицы для claims, внешних логинов и токенов. |

Поле `FullName` добавлено в `AspNetUsers`:

| Поле | Тип | Описание |
|------|-----|----------|
| `FullName` | `text` | Полное имя пользователя. |

---

## Складская иерархия

### Таблица `Warehouses`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Уникальный идентификатор склада. |
| `Name` | `text` | `NOT NULL` | Название склада. |
| `Address` | `text` | `NULL` | Адрес склада. |
| `ContactPhone` | `text` | `NULL` | Контактный телефон. |

---

### Таблица `Zones`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор зоны. |
| `Name` | `text` | `NOT NULL` | Название зоны. |
| `Type` | `int` | `NOT NULL` | Тип зоны (Enum `ZoneType`: 0 — Normal, 1 — Fridge, 2 — Freezer). |
| `WarehouseId` | `int` | `FK`, `NOT NULL` | Ссылка на склад (`Warehouses.Id`). |

**Индексы:** нет дополнительных.

---

### Таблица `Racks`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор стеллажа. |
| `Code` | `text` | `NOT NULL` | Буквенное обозначение (например, "A"). |
| `ZoneId` | `int` | `FK`, `NOT NULL` | Ссылка на зону (`Zones.Id`). |

**Индексы:** `UQ` на (`ZoneId`, `Code`) — гарантирует уникальность кода стеллажа в пределах зоны.

---

### Таблица `Shelves`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор полки. |
| `Number` | `int` | `NOT NULL` | Номер полки (порядковый). |
| `RackId` | `int` | `FK`, `NOT NULL` | Ссылка на стеллаж (`Racks.Id`). |

**Индексы:** `UQ` на (`RackId`, `Number`) — уникальность номера полки в пределах стеллажа.

---

### Таблица `Cells`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор ячейки. |
| `Code` | `text` | `NOT NULL` | Код ячейки (например, "A-1-1" или просто номер). |
| `ShelfId` | `int` | `FK`, `NOT NULL` | Ссылка на полку (`Shelves.Id`). |
| `IsOccupied` | `boolean` | `NOT NULL`, по умолчанию `false` | Флаг занятости (упрощённо). |

**Индексы:** `UQ` на (`ShelfId`, `Code`) — уникальность кода ячейки в пределах полки.

---

## Товары и партии

### Таблица `Products`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор товара. |
| `Name` | `text` | `NOT NULL` | Название товара. |
| `Manufacturer` | `text` | `NULL` | Производитель. |
| `Supplier` | `text` | `NULL` | Поставщик по умолчанию. |
| `Category` | `text` | `NULL` | Категория. |
| `Unit` | `text` | `NOT NULL`, по умолчанию `"шт"` | Единица измерения. |
| `MinStockThreshold` | `numeric(18,2)` | `NOT NULL`, по умолчанию `0` | Минимальный порог остатка. |

**Индексы:** для ускорения поиска рекомендуется индекс на `Name` и `Category`.

---

### Таблица `Batches`

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор партии. |
| `ProductId` | `int` | `FK`, `NOT NULL` | Ссылка на товар (`Products.Id`). |
| `Quantity` | `int` | `NOT NULL` | Текущее доступное количество. |
| `ReservedQuantity` | `int` | `NOT NULL`, по умолчанию `0` | Зарезервированное количество под заказы. |
| `PurchasePrice` | `numeric(18,2)` | `NOT NULL` | Закупочная цена за единицу. |
| `ProductionDate` | `timestamp` | `NOT NULL` | Дата производства. |
| `ExpiryDate` | `timestamp` | `NOT NULL` | Дата истечения срока годности. |
| `ReceivedDate` | `timestamp` | `NOT NULL` | Дата поступления на склад. |
| `CellId` | `int` | `FK`, `NOT NULL` | Ссылка на ячейку (`Cells.Id`). |

**Индексы:**
- На `ProductId` для поиска партий товара.
- На `ExpiryDate` для фоновых проверок.
- На `CellId` для поиска партии по ячейке (уникальность не требуется, так как ячейка может быть пустой, но бизнес-логика гарантирует, что в одной ячейке не более одной партии).

**Ограничение:** `ReservedQuantity` всегда <= `Quantity`.

---

## Приёмка (Receipt)

### Таблица `Receipts`

Документ ожидаемой поставки.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор документа. |
| `Supplier` | `text` | `NULL` | Название поставщика. |
| `Comment` | `text` | `NULL` | Примечание (например, расхождения). |
| `Status` | `int` | `NOT NULL` | Статус (Enum `ReceiptStatus`: 0 — Pending, 1 — Received, 2 — Rejected). |
| `CreatedBy` | `text` | `NOT NULL` | Ссылка на пользователя (`AspNetUsers.Id`) — кто создал. |
| `CreatedAt` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Дата создания. |
| `ReceivedAt` | `timestamp` | `NULL` | Дата фактической приёмки (заполняется при подтверждении). |
| `RejectedAt` | `timestamp` | `NULL` | Дата отклонения (заполняется при отклонении). |

**Индексы:** на `Status`, `CreatedBy`, `CreatedAt`.

---

### Таблица `ReceiptLines`

Ожидаемые позиции в документе приёмки.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор позиции. |
| `ReceiptId` | `int` | `FK`, `NOT NULL` | Ссылка на документ (`Receipts.Id`). |
| `ProductId` | `int` | `FK`, `NOT NULL` | Ожидаемый товар (`Products.Id`). |
| `ExpectedQuantity` | `int` | `NOT NULL` | Ожидаемое количество. |
| `ActualQuantity` | `int` | `NULL` | Фактическое количество (заполняется при приёмке). |
| `CellId` | `int` | `NULL` | Ячейка, куда размещена партия (заполняется при приёмке). |
| `ExpiryDate` | `timestamp` | `NULL` | Срок годности (заполняется при приёмке). |
| `PurchasePrice` | `numeric(18,2)` | `NULL` | Закупочная цена (заполняется при приёмке). |

**Индексы:** на `ReceiptId`, `ProductId`.

**Бизнес-логика:** при подтверждении приёмки для каждой строки создаётся партия (`Batch`) со значениями `ActualQuantity`, `CellId`, `ExpiryDate`, `PurchasePrice`. Если `ActualQuantity` отличается от `ExpectedQuantity`, это фиксируется в `Receipt.Comment`.

---

## Заявки магазина (SupplyRequest)

### Таблица `SupplyRequests`

Заявка от магазина на поставку товаров.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор заявки. |
| `StoreName` | `text` | `NOT NULL` | Название магазина. |
| `CreatedBy` | `text` | `NOT NULL` | Ссылка на пользователя (`AspNetUsers.Id`) — создатель (StoreDirector). |
| `Status` | `int` | `NOT NULL` | Статус (Enum `SupplyRequestStatus`: 0 — Draft, 1 — Submitted, 2 — Approved, 3 — Completed, 4 — Rejected). |
| `CreatedAt` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Дата создания. |
| `SubmittedAt` | `timestamp` | `NULL` | Дата отправки на рассмотрение. |
| `ApprovedAt` | `timestamp` | `NULL` | Дата одобрения. |
| `RejectedAt` | `timestamp` | `NULL` | Дата отклонения. |
| `CompletedAt` | `timestamp` | `NULL` | Дата завершения (когда отгрузка выполнена). |

**Индексы:** на `Status`, `CreatedBy`, `StoreName`.

---

### Таблица `SupplyRequestLines`

Позиции заявки (какие товары и в каком количестве запрошены).

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор позиции. |
| `SupplyRequestId` | `int` | `FK`, `NOT NULL` | Ссылка на заявку (`SupplyRequests.Id`). |
| `ProductId` | `int` | `FK`, `NOT NULL` | Товар (`Products.Id`). |
| `RequestedQuantity` | `int` | `NOT NULL` | Запрошенное количество. |

**Индексы:** на `SupplyRequestId`, `ProductId`.

---

## Планы отгрузки (SupplyOrder)

### Таблица `SupplyOrders`

Документ на отгрузку товаров со склада (может быть создан на основе заявки или без неё).

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор документа отгрузки. |
| `SupplyRequestId` | `int` | `FK`, `NULL` | Ссылка на заявку (`SupplyRequests.Id`), если заказ создан на основе заявки. |
| `CreatedBy` | `text` | `NOT NULL` | Ссылка на пользователя (`AspNetUsers.Id`) — кто создал (Manager). |
| `Status` | `int` | `NOT NULL` | Статус (Enum `SupplyOrderStatus`: 0 — Draft, 1 — Confirmed, 2 — Shipped). |
| `CreatedAt` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Дата создания. |
| `ConfirmedAt` | `timestamp` | `NULL` | Дата подтверждения (резервирование выполнено). |
| `ShippedAt` | `timestamp` | `NULL` | Дата отгрузки (списание выполнено). |

**Индексы:** на `Status`, `SupplyRequestId`, `CreatedBy`.

---

### Таблица `SupplyOrderLines`

Позиции отгрузки (какие товары и в каком количестве отгружаются).

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор позиции. |
| `SupplyOrderId` | `int` | `FK`, `NOT NULL` | Ссылка на отгрузку (`SupplyOrders.Id`). |
| `ProductId` | `int` | `FK`, `NOT NULL` | Товар (`Products.Id`). |
| `Quantity` | `int` | `NOT NULL` | Количество для отгрузки. |

**Индексы:** на `SupplyOrderId`, `ProductId`.

---

## Резервирование (Reservation)

### Таблица `Reservations`

Фиксирует, какие партии зарезервированы под конкретный заказ на отгрузку.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор записи резерва. |
| `BatchId` | `int` | `FK`, `NOT NULL` | Ссылка на партию (`Batches.Id`). |
| `SupplyOrderId` | `int` | `FK`, `NOT NULL` | Ссылка на заказ отгрузки (`SupplyOrders.Id`). |
| `Quantity` | `int` | `NOT NULL` | Зарезервированное количество из этой партии. |
| `CreatedAt` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Время создания резерва. |

**Индексы:**
- Уникальный индекс на (`BatchId`, `SupplyOrderId`) — чтобы не было дублирования резерва для одной партии под один заказ (хотя несколько записей могут быть, если резервирование частями, но обычно одна запись на партию).
- Индекс на `SupplyOrderId` для быстрого поиска резервов по заказу.
- Индекс на `BatchId` для обновления `ReservedQuantity`.

**Бизнес-логика:**
- При подтверждении заказа (`SupplyOrder.Confirm`) для каждой позиции ищутся доступные партии, создаются записи в `Reservations`, и суммарное зарезервированное количество обновляется в `Batch.ReservedQuantity` (атомарно в одной транзакции).
- При отгрузке (`SupplyOrder.Ship`) записи удаляются (или помечаются как исполненные), а `Batch.Quantity` уменьшается, `ReservedQuantity` уменьшается на соответствующие величины.

---

## Аудит движений

### Таблица `InventoryTransactions`

Журнал всех изменений остатков и перемещений.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор записи аудита. |
| `BatchId` | `int` | `FK`, `NOT NULL` | Ссылка на партию (`Batches.Id`). |
| `QuantityChange` | `int` | `NOT NULL` | Изменение количества (положительное — приход, отрицательное — расход). |
| `TransactionType` | `int` | `NOT NULL` | Тип операции (Enum `TransactionType`: 0 — In, 1 — Out, 2 — Move, 3 — WriteOff). |
| `DocumentId` | `int` | `NULL` | Идентификатор документа-инициатора (`Receipts.Id` для прихода, `SupplyOrders.Id` для отгрузки). |
| `UserId` | `text` | `NOT NULL` | Ссылка на пользователя (`AspNetUsers.Id`). |
| `Timestamp` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Время операции. |
| `OldCellId` | `int` | `NULL` | Для перемещений — исходная ячейка (`Cells.Id`). |
| `NewCellId` | `int` | `NULL` | Для перемещений — целевая ячейка (`Cells.Id`). |

**Индексы:** на `BatchId`, `TransactionType`, `Timestamp`, `UserId`, `DocumentId` для быстрых фильтров.

---

## Уведомления

### Таблица `Notifications`

Системные уведомления для пользователей.

| Поле | Тип | Ограничение | Описание |
|------|-----|-------------|----------|
| `Id` | `int` | `PK`, `NOT NULL` | Идентификатор уведомления. |
| `UserId` | `text` | `FK`, `NOT NULL` | Ссылка на пользователя (`AspNetUsers.Id`), кому адресовано. |
| `Title` | `text` | `NOT NULL` | Заголовок уведомления. |
| `Message` | `text` | `NOT NULL` | Текст уведомления. |
| `IsRead` | `boolean` | `NOT NULL`, по умолчанию `false` | Прочитано ли. |
| `CreatedAt` | `timestamp` | `NOT NULL`, по умолчанию `CURRENT_TIMESTAMP` | Время создания. |

**Индексы:** на `UserId`, `IsRead`, `CreatedAt`.

**Генерация:** фоновый сервис создаёт уведомления для менеджеров при:
- `ExpiryDate - Today <= 3` (просрочка близка).
- `Batch.Quantity - Batch.ReservedQuantity < Product.MinStockThreshold` (низкий остаток).

---

## Связи между таблицами (полная ER-диаграмма в текстовом виде)

```
Identity (AspNetUsers) ───┬─── SupplyRequests.CreatedBy
                          ├─── SupplyOrders.CreatedBy
                          ├─── Receipts.CreatedBy
                          ├─── InventoryTransactions.UserId
                          └─── Notifications.UserId
Warehouses 1───* Zones
Zones 1───* Racks
Racks 1───* Shelves
Shelves 1───* Cells
Cells 1───0..1 Batches (одна партия в одной ячейке, одна ячейка может быть пустой или занята одной партией)

Products 1───* Batches
Products 1───* ReceiptLines
Products 1───* SupplyRequestLines
Products 1───* SupplyOrderLines

Batches 1───* InventoryTransactions
Batches 1───* Reservations
Batches 1───0..1 ReceiptLine (связь при создании партии из ReceiptLine не обязательна, но можно добавить)

SupplyRequests 1───* SupplyRequestLines
SupplyRequests 1───0..1 SupplyOrders (один заказ на одну заявку)

SupplyOrders 1───* SupplyOrderLines
SupplyOrders 1───* Reservations

Receipts 1───* ReceiptLines
ReceiptLines 1───1 Batch (при приёмке создаётся партия, связь можно хранить в ReceiptLine через BatchId, но в ТЗ упрощено — партия создаётся и привязывается к Cell, а связь с ReceiptLine не обязательна, но для аудита можно добавить поле BatchId в ReceiptLine)

InventoryTransactions.DocumentId может ссылаться на Receipts.Id или SupplyOrders.Id (полиморфная связь, но без FK-ограничений)
```


---

## Примечания по индексам и производительности

- Внешние ключи создаются с каскадным удалением (`ON DELETE CASCADE`) для упрощения (кроме Identity, где политики стандартные).
- Для таблиц с большим объёмом данных (InventoryTransactions) рекомендуется партиционирование по дате или архивация.
- Для поиска по `ExpiryDate` и `MinStockThreshold` фоновым сервисом используются индексы на соответствующие поля.
- Для `Reservations` комбинированный индекс на `(BatchId, SupplyOrderId)` гарантирует уникальность и ускоряет поиск.

---

## Заключение

Данная схема полностью покрывает все сущности, описанные в ТЗ, включая:
- Складскую иерархию
- Товары и партии
- Приёмку (Receipt)
- Заявки магазина (SupplyRequest)
- Отгрузки (SupplyOrder)
- Резервирование (Reservation)
- Аудит (InventoryTransaction)
- Уведомления (Notification)

Она является основой для всех последующих итераций разработки. Миграции будут добавлять эти таблицы поэтапно, но в документации они отражены полностью для целостности архитектурного описания.