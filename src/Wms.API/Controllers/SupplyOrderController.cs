using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Supply;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Documents;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplyOrderController(ISupplyOrderService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<ActionResult<SupplyOrderDto>> Create([FromBody] CreateSupplyOrderRequest request,
        CancellationToken cancellationToken)
    {
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplyOrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await service.GetAllWithLinesAsync(cancellationToken);
        return Ok(mapper.Map<IEnumerable<SupplyOrderDto>>(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplyOrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await service.GetByIdWithDetailsAsync(id, cancellationToken);

        if (order == null)
            return NotFound();

        return Ok(mapper.Map<SupplyOrderDto>(order));
    }

    [HttpPut("{id}/confirm")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        await service.ConfirmOrderAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/ship")]
    [Authorize(Roles = "Manager,Chief,Worker")]
    public async Task<IActionResult> Ship(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await service.ShipOrderAsync(id, userId, cancellationToken);
        return NoContent();
    }
}