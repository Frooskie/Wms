using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;
using System.Security.Claims;
using Wms.API.DTOs.Receipts;
using Wms.Core.Enums;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceiptController(IReceiptService receiptService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<ActionResult<ReceiptDto>> CreateReceipt([FromBody] CreateReceiptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var receipt = new Receipt
        {
            Supplier = request.Supplier,
            CreatedBy = userId,
            Status = ReceiptStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        var lines = request.Lines.Select(l => new ReceiptLine
        {
            ProductId = l.ProductId,
            ExpectedQuantity = l.ExpectedQuantity
        }).ToList();

        var created = await receiptService.CreateReceiptAsync(receipt, lines, cancellationToken);
        var dto = mapper.Map<ReceiptDto>(created);
        return CreatedAtAction(nameof(GetReceipt), new { id = created.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetReceipts(CancellationToken cancellationToken)
    {
        var receipts = await receiptService.GetAllReceiptsWithLinesAsync(cancellationToken);
        var dtos = mapper.Map<IEnumerable<ReceiptDto>>(receipts);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReceiptDto>> GetReceipt(int id, CancellationToken cancellationToken)
    {
        var receipt = await receiptService.GetReceiptWithLinesAsync(id, cancellationToken);
        if (receipt == null)
            return NotFound();
        var dto = mapper.Map<ReceiptDto>(receipt);
        return Ok(dto);
    }

    [HttpPut("{id}/receive")]
    [Authorize(Roles = "Manager,Chief,Worker")]
    public async Task<IActionResult> ReceiveReceipt(int id, [FromBody] ReceiveReceiptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var receiveLines = request.Lines.Select(l => (
            l.ProductId,
            l.ActualQuantity,
            l.CellId,
            l.ExpiryDate,
            l.PurchasePrice
        )).ToList();

        await receiptService.ReceiveReceiptAsync(id, receiveLines, userId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Manager,Chief")]
    public async Task<IActionResult> RejectReceipt(int id, CancellationToken cancellationToken)
    {
        await receiptService.RejectReceiptAsync(id, cancellationToken);
        return NoContent();
    }
}