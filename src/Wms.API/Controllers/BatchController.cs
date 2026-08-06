using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Batches;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BatchController(IBatchService batchService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBatches(
        [FromQuery] int? productId,
        [FromQuery] int? cellId,
        [FromQuery] DateTime? expiryFrom,
        [FromQuery] DateTime? expiryTo,
        CancellationToken cancellationToken)
    {
        var batches =
            await batchService.GetBatchesWithFiltersAsync(productId, cellId, expiryFrom, expiryTo, cancellationToken);
        var dtos = mapper.Map<IEnumerable<BatchDto>>(batches);
        return Ok(dtos);
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

    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        
        var batch = mapper.Map<Batch>(request);
        await batchService.CreateBatchAsync(batch, userId, null, cancellationToken);

        var dto = mapper.Map<BatchDto>(batch);
        return CreatedAtAction(nameof(GetBatch), new { id = batch.Id }, dto);
    }

    [HttpPut("{id}/move")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> MoveBatch(int id, [FromBody] MoveBatchRequest request,
        CancellationToken cancellationToken)
    {
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