using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Batches;
using Wms.API.Extensions;
using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Inventory;

namespace Wms.API.Controllers;

/// <summary>Управление партиями товаров.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [HttpGet]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBatch(int id, CancellationToken cancellationToken)
    {
        var batch = await batchService.GetByIdAsync(id, cancellationToken);
        if (batch == null)
            return NotFound();

        var dto = mapper.Map<BatchDto>(batch);
        return Ok(dto);
    }

    /// <summary>Создать новую партию (только Manager/Chief).</summary>
    /// <returns>Созданная партия.</returns>
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
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
    /// <response code="204">Успешное перемещение.</response>
    /// <response code="400">Целевая ячейка занята или не существует.</response>
    /// <response code="404">Партия не найдена.</response>
    [HttpPut("{id}/move")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> MoveBatch(int id, [FromBody] MoveBatchRequest request,
        CancellationToken cancellationToken)
    {
        await moveBatchValidator.ValidateAndThrowAsync(request, cancellationToken);
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        await batchService.MoveBatchAsync(id, request.CellId, userId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> DeleteBatch(int id, CancellationToken cancellationToken)
    {
        await batchService.DeleteBatchAsync(id, cancellationToken);
        return NoContent();
    }
}