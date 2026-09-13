using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Batches;
using Wms.API.Extensions;
using Wms.Core.Constants;
using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Inventory;

namespace Wms.API.Controllers;

/// <summary>Управление партиями товаров.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
public class BatchController(
    IBatchService batchService,
    IMapper mapper,
    IValidator<CreateBatchRequest> createBatchValidator,
    IValidator<MoveBatchRequest> moveBatchValidator,
    IValidator<BatchFilterDto> filterBatchValidator)
    : ControllerBase
{
    /// <summary>Получить список партий с фильтрацией и пагинацией.</summary>
    /// <returns>Страница со списком партий и информацией о пагинации.</returns>
    /// <response code="200">OK. Возвращает страницу с партиями.</response>
    /// <response code="400">Ошибка валидации параметров фильтрации.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<BatchDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PagedResult<BatchDto>>> GetBatches(
        [FromQuery] BatchFilterDto filter,
        CancellationToken cancellationToken)
    {
        await filterBatchValidator.ValidateAndThrowAsync(filter, cancellationToken);

        var pagedResult = await batchService.GetPagedBatchesWithFiltersAsync(
            filter.ProductId,
            filter.CellId,
            filter.ExpiryFrom,
            filter.ExpiryTo,
            filter.PageNumber,
            filter.PageSize,
            cancellationToken);

        var dtoItems = mapper.Map<IEnumerable<BatchDto>>(pagedResult.Items);
        var response = new PagedResult<BatchDto>(
            dtoItems,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize);

        return Ok(response);
    }

    /// <summary>Получить партию по идентификатору.</summary>
    /// <param name="id">Идентификатор партии.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>DTO партии.</returns>
    /// <response code="200">OK. Возвращает партию.</response>
    /// <response code="404">Партия не найдена.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetBatch(int id, CancellationToken cancellationToken)
    {
        var batch = await batchService.GetByIdAsync(id, cancellationToken);
        if (batch == null)
            throw new NotFoundException(ErrorMessages.Batch.NotFoundFormat(id));

        var dto = mapper.Map<BatchDto>(batch);
        return Ok(dto);
    }

    /// <summary>Создать новую партию (только Manager/Chief).</summary>
    /// <param name="request">Данные для создания партии.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданная партия.</returns>
    /// <response code="201">Партия успешно создана. Возвращает DTO созданной партии.</response>
    /// <response code="400">Ошибка валидации запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Недостаточно прав (требуется Manager или Chief).</response>
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BatchDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest request,
        CancellationToken cancellationToken)
    {
        await createBatchValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var batch = mapper.Map<Batch>(request);
        await batchService.CreateBatchAsync(batch, userId, null, cancellationToken);

        var dto = mapper.Map<BatchDto>(batch);
        return CreatedAtAction(nameof(GetBatch), new { id = batch.Id }, dto);
    }

    /// <summary>Переместить партию в другую ячейку (только Manager/Chief).</summary>
    /// <param name="id">Идентификатор партии.</param>
    /// <param name="request">Данные для перемещения (целевая ячейка).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="204">Успешное перемещение.</response>
    /// <response code="400">Целевая ячейка занята или не существует.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Недостаточно прав (требуется Manager или Chief).</response>
    /// <response code="404">Партия не найдена.</response>
    [HttpPut("{id}/move")]
    [Authorize(Roles = "Manager,Chief")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MoveBatch(int id, [FromBody] MoveBatchRequest request,
        CancellationToken cancellationToken)
    {
        await moveBatchValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        await batchService.MoveBatchAsync(id, request.CellId, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>Удалить партию (только Manager/Chief).</summary>
    /// <param name="id">Идентификатор партии.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="204">Партия успешно удалена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Недостаточно прав (требуется Manager или Chief).</response>
    /// <response code="404">Партия не найдена.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteBatch(int id, CancellationToken cancellationToken)
    {
        await batchService.DeleteBatchAsync(id, cancellationToken);
        return NoContent();
    }
}