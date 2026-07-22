using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.API.DTOs.Batches;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

[Authorize]
public class BatchController(IBatchService batchService, IMapper mapper)
    : BaseApiController<Batch, BatchDto, CreateBatchRequest, UpdateBatchRequest>(batchService, mapper)
{
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<BatchDto>>> GetBatches(
        [FromQuery] int? productId,
        [FromQuery] int? cellId,
        [FromQuery] DateTime? expiryFrom,
        [FromQuery] DateTime? expiryTo,
        CancellationToken cancellationToken)
    {
        var batches = await batchService.GetBatchesWithFiltersAsync(productId, cellId, expiryFrom, expiryTo, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<BatchDto>>(batches);
        return Ok(dtos);
    }
    
    public override async Task<IActionResult> Update(int id, [FromBody] UpdateBatchRequest request, CancellationToken cancellationToken)
    {
        var existing = await batchService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound();

        _mapper.Map(request, existing);
        await batchService.UpdateAsync(existing, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("{id}/move")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> MoveBatch(int id, [FromBody] MoveBatchRequest request, CancellationToken cancellationToken)
    {
        await batchService.MoveBatchAsync(id, request.CellId, cancellationToken);
        return NoContent();
    }
    
    protected override object GetEntityId(Batch entity) => entity.Id;
}