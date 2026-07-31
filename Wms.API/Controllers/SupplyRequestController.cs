using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Supply;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplyRequestController(ISupplyRequestService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "StoreDirector")]
    public async Task<ActionResult<SupplyRequestDto>> Create([FromBody] CreateSupplyRequestRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var req = new SupplyRequest
        {
            StoreName = request.StoreName,
            CreatedBy = userId,
            Status = SupplyRequestStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
        var lines = request.Lines.Select(l => new SupplyRequestLine
        {
            ProductId = l.ProductId,
            RequestedQuantity = l.RequestedQuantity
        }).ToList();

        var created = await service.CreateSupplyRequestAsync(req, lines, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<SupplyRequestDto>(created));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplyRequestDto>>> GetAll([FromQuery] string? status, [FromQuery] string? createdBy, CancellationToken cancellationToken)
    {
        var requests = await service.GetAllWithLinesAsync(cancellationToken);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SupplyRequestStatus>(status, true, out var statusEnum))
            requests = requests.Where(r => r.Status == statusEnum);
        if (!string.IsNullOrEmpty(createdBy))
            requests = requests.Where(r => r.CreatedBy == createdBy);
        return Ok(mapper.Map<IEnumerable<SupplyRequestDto>>(requests));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplyRequestDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var req = await service.GetByIdWithLinesAsync(id, cancellationToken);
        if (req == null)
            return NotFound();
        return Ok(mapper.Map<SupplyRequestDto>(req));
    }

    [HttpPut("{id}/submit")]
    public async Task<IActionResult> Submit(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await service.SubmitAsync(id, userId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        await service.ApproveAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        await service.RejectAsync(id, cancellationToken);
        return NoContent();
    }
}