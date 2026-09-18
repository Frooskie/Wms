using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Common;
using Wms.API.DTOs.Supply;
using Wms.API.Extensions;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Documents;

namespace Wms.API.Controllers;

/// <summary>Управление заказами на отгрузку.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SupplyOrderController(
    ISupplyOrderService service,
    IMapper mapper,
    IValidator<CreateSupplyOrderRequest> createSupplyOrderValidator)
    : ControllerBase
{
    /// <summary>Создать новый заказ на отгрузку.</summary>
    /// <param name="request">Данные для создания.</param>
    /// <response code="201">Заказ создан. Возвращает созданный объект.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WmsProblemDetails))]
    public async Task<ActionResult<SupplyOrderDto>> Create(
        [FromBody] CreateSupplyOrderRequest request,
        CancellationToken cancellationToken)
    {
        await createSupplyOrderValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var order = new SupplyOrder
        {
            SupplyRequestId = request.SupplyRequestId,
            CreatedBy = userId,
            Status = SupplyOrderStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
        var lines = request.Lines.Select(l => new SupplyOrderLine
        {
            ProductId = l.ProductId,
            Quantity = l.Quantity
        }).ToList();

        var created = await service.CreateOrderAsync(order, lines, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<SupplyOrderDto>(created));
    }

    /// <summary>Получить все заказы.</summary>
    /// <response code="200">Список заказов.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SupplyOrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await service.GetAllWithLinesAsync(cancellationToken);
        return Ok(mapper.Map<IEnumerable<SupplyOrderDto>>(orders));
    }

    /// <summary>Получить заказ по идентификатору.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <response code="200">Заказ найден.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WmsProblemDetails))]
    public async Task<ActionResult<SupplyOrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await service.GetByIdWithDetailsAsync(id, cancellationToken);

        if (order == null)
            throw new NotFoundException(ErrorMessages.SupplyOrder.NotFoundFormat(id));

        return Ok(mapper.Map<SupplyOrderDto>(order));
    }

    /// <summary>Подтвердить заказ — выполнить резервирование товаров.</summary>
    /// <remarks>
    /// Для каждой позиции заказа система ищет доступные партии (Quantity - ReservedQuantity).
    /// Если суммарного доступного количества достаточно, создаются записи Reservation и обновляется ReservedQuantity.
    /// Если не хватает хотя бы для одной позиции, заказ не подтверждается.
    /// </remarks>
    /// <param name="id">Идентификатор заказа.</param>
    /// <response code="204">Резервирование выполнено.</response>
    /// <response code="400">Недостаточно остатков или заказ уже подтверждён.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpPut("{id}/confirm")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WmsProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WmsProblemDetails))]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        await service.ConfirmOrderAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Отгрузить заказ — списать остатки и удалить резервы.</summary>
    /// <remarks>
    /// Заказ должен быть в статусе Confirmed. Для каждой партии списывается зарезервированное количество,
    /// создаётся транзакция Out, резервы удаляются. Если партия становится пустой, ячейка освобождается.
    /// </remarks>
    /// <param name="id">Идентификатор заказа.</param>
    /// <response code="204">Отгрузка выполнена.</response>
    /// <response code="400">Заказ не в статусе Confirmed.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpPut("{id}/ship")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief + "," + Roles.Worker)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WmsProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WmsProblemDetails))]
    public async Task<IActionResult> Ship(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await service.ShipOrderAsync(id, userId, cancellationToken);
        
        return NoContent();
    }
}