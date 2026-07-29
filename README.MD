# Warehouse Management System (WMS)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Status](https://img.shields.io/badge/status-in%20development-yellow)]()

## 📚 Документация

Подробное описание архитектуры, бизнес-логики, API и развёртывания находится в папке [`/docs`](docs/) репозитория. В ней содержатся следующие материалы:

| Файл | Описание |
|------|----------|
| [`architecture.md`](docs/architecture.md) | Описание Clean Architecture, слоёв проекта, зависимостей и взаимодействия между ними. |
| [`database-schema.md`](docs/database-schema.md) | Схема базы данных со связями сущностей, описание таблиц и индексов. |
| [`business-rules.md`](docs/business-rules.md) | Ключевые бизнес-правила: резервирование, списание, перемещение, уведомления, права доступа. |

Кроме того, при запущенном приложении доступна **интерактивная документация API** через Swagger UI по адресу:

https://localhost:5001/swagger

(порт может отличаться в зависимости от настроек запуска)

---

Если вы открыли этот README на GitHub, все ссылки выше ведут на соответствующие файлы внутри репозитория. Для просмотра документации в удобном формате вы можете также использовать [GitHub Pages](https://pages.github.com/) (если будет настроен).

## 📝 Описание проекта

**WMS (Warehouse Management System)** — это RESTful API для управления складскими операциями: приёмка товаров, отгрузка, резервирование, контроль сроков годности и аудит движений. Проект разработан на **.NET 8** с соблюдением **Clean Architecture** и предназначен для изучения современных подходов в разработке на C#.

Проект реализует ролевую модель (Chief, Manager, Worker, StoreDirector) с аутентификацией через JWT и поддерживает:
- Управление складской иерархией (склады → зоны → стеллажи → полки → ячейки).
- Учёт товаров и партий с отслеживанием остатков, резервов и сроков годности.
- Приёмку, отгрузку, перемещение партий и списание просрочки.
- Полный аудит всех изменений через журнал транзакций.
- Автоматические уведомления о низких остатках и приближающейся просрочке.

---

## 🏗️ Архитектура

Проект построен на **Clean Architecture** и состоит из четырёх слоёв:

- **Warehouse.Core** — доменные сущности, интерфейсы репозиториев и сервисов, перечисления, исключения. (не зависит от внешних фреймворков)
- **Warehouse.Infrastructure** — реализация репозиториев, DbContext, миграции, настройка Identity.
- **Warehouse.Services** — бизнес-логика (сервисы), фоновые задачи.
- **Warehouse.API** — контроллеры, DTO, AutoMapper, Swagger, настройка DI и JWT.

Зависимости строго направлены от API к Services, от Services к Core, от Infrastructure к Core. **Core** не имеет зависимостей от других слоёв.

---

## 🗄️ Модель данных (основные сущности)

- **Product** — шаблон товара (название, производитель, поставщик, категория, единица измерения, порог остатка).
- **Warehouse → Zone → Rack → Shelf → Cell** — иерархия размещения.
- **Batch** — партия товара (Product, Quantity, ReservedQuantity, цены, даты, CellId).
- **Reservation** — резервы под заказы (BatchId, SupplyOrderId, Quantity).
- **SupplyRequest** — заявка магазина (статусы Draft, Submitted, Approved, Completed, Rejected).
- **SupplyOrder** — заказ на отгрузку (статусы Draft, Confirmed, Shipped), создаёт резервы и списывает остатки.
- **Receipt** — приёмка товара (ожидаемые и фактические позиции, статусы Pending, Received, Rejected).
- **InventoryTransaction** — аудит движений (BatchId, QuantityChange, тип, документ, пользователь, время, ячейки).
- **Notification** — уведомления пользователей.

---

## 📦 Основные возможности

- 📋 **Управление складской иерархией**  
  Склады → Зоны (Normal / Fridge / Freezer) → Стеллажи → Полки → Ячейки.

- 🏷️ **Товары и партии**  
  Справочник товаров с порогами минимального остатка. Каждая партия привязана к конкретной ячейке и содержит информацию о количестве, закупочной цене, сроках производства и годности.

- 📄 **Приёмка товаров**  
  Создание ожидаемых поставок, подтверждение фактического прихода с выбором ячейки и созданием партий. Расхождения фиксируются в комментариях.

- 📦 **Резервирование и отгрузка**  
  Менеджер создаёт заказы на отгрузку, система резервирует товары из доступных партий (с учётом уже зарезервированного количества). Подтверждение отгрузки списывает остатки и удаляет резервы.

- 📊 **Аудит всех движений**  
  Каждое изменение остатков (приход, расход, перемещение, списание) фиксируется в таблице транзакций с указанием пользователя, документа и времени.

- 🔔 **Автоматические уведомления**  
  Фоновый сервис ежечасно проверяет:
   - сроки годности (за 3 дня до истечения — уведомление менеджерам);
   - остатки ниже порога `MinStockThreshold` — уведомление менеджерам.

- 🔐 **Ролевая модель**
   - **Chief** — полный доступ: CRUD пользователей, просмотр всех транзакций и уведомлений.
   - **Manager** — управление товарами, складской иерархией, приёмками, отгрузками, перемещениями партий, списанием просрочки.
   - **Worker** — подтверждение приёмки и отгрузки, просмотр данных.
   - **StoreDirector** — создание заявок на поставку и просмотр их статусов.

---

## 🛠️ Технологический стек

| Компонент | Технология |
|-----------|------------|
| **Платформа** | .NET 8 (C# 12) |
| **База данных** | PostgreSQL 16 |
| **ORM** | Entity Framework Core 8 (Npgsql) |
| **Аутентификация** | ASP.NET Core Identity + JWT Bearer |
| **Маппинг** | AutoMapper |
| **Валидация** | FluentValidation |
| **Логирование** | Serilog |
| **Документация API** | Swashbuckle (Swagger) |
| **Тестирование** | xUnit, Moq |
| **Среда разработки** | JetBrains Rider 2025.3.2 |

---

## 📁 Структура решения

```
Warehouse/
├── src/
│ ├── Warehouse.API/ # Контроллеры, DTO, Middleware, DI, Program.cs
│ ├── Warehouse.Core/ # Entities, Enums, Interfaces (Repositories, Services)
│ ├── Warehouse.Infrastructure/# DbContext, Repositories, Migrations, Identity
│ └── Warehouse.Services/ # Business logic services, Background services
├── tests/ # Unit-тесты
├── docs/ # Подробная документация
│ ├── architecture.md
│ ├── database-schema.md
│ ├── api-endpoints.md
│ ├── business-rules.md
│ ├── deployment.md
│ └── decisions.md
├── README.md
└── LICENSE
```

## 🚀 Быстрый старт

### Требования
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/download/) (или Docker-контейнер)
- [JetBrains Rider 2025.3.2](https://www.jetbrains.com/rider/) (рекомендуется) или Visual Studio 2022

### Шаги для запуска

1. **Клонируйте репозиторий**
2. **Настройте строку подключения**
   В файле src/Warehouse.API/appsettings.json укажите вашу PostgreSQL-строку:
   ```
   git clone https://github.com/yourusername/warehouse.git
   cd warehouse
   ```
3. **Примените миграции**
   В корне решения выполните:
   ```bash
   dotnet ef database update --project src/Warehouse.Infrastructure --startup-project src/Warehouse.API
   ```
4. **Запустите приложение**
   ```bash
   cd src/Warehouse.API
   dotnet run
   ```

### Тестовые данные

При первом запуске автоматически создаются роли (Chief, Manager, Worker, StoreDirector) и пользователь Chief:

- Email: chief@warehouse.com
- Пароль: Chief@123