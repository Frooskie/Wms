using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Receipts;
using Wms.API.Extensions;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Documents;

namespace Wms.API.Controllers;

/// <summary>Управление приёмкой товаров.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ReceiptController(
    IReceiptService receiptService,
    IMapper mapper,
    IValidator<CreateReceiptRequest> createReceiptValidator,
    IValidator<ReceiveReceiptRequest> receiveReceiptValidator)
    : ControllerBase
{
    /// <summary>Создать документ приёмки.</summary>
    /// <response code="201">Документ приёмки создан.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReceiptDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ReceiptDto>> CreateReceipt(
        [FromBody] CreateReceiptRequest request,
        CancellationToken cancellationToken)
    {
        await createReceiptValidator.ValidateAndThrowAsync(request, cancellationToken);

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

    /// <summary>Получить все документы приёмки.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="200">Список документов приёмки.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReceiptDto>))]
    public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetReceipts(CancellationToken cancellationToken)
    {
        var receipts = await receiptService.GetAllReceiptsWithLinesAsync(cancellationToken);
        var dtos = mapper.Map<IEnumerable<ReceiptDto>>(receipts);

        return Ok(dtos);
    }

    /// <summary>Получить документ приёмки по идентификатору.</summary>
    /// <param name="id">Идентификатор документа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="200">Документ найден.</response>
    /// <response code="404">Документ не найден.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReceiptDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ReceiptDto>> GetReceipt(int id, CancellationToken cancellationToken)
    {
        var receipt = await receiptService.GetReceiptWithLinesAsync(id, cancellationToken);

        if (receipt == null)
            throw new NotFoundException(ErrorMessages.Receipt.NotFoundFormat(id));

        var dto = mapper.Map<ReceiptDto>(receipt);
        return Ok(dto);
    }

    /// <summary>Подтвердить приёмку товара.</summary>
    /// <remarks>
    /// Если фактическое количество отличается от ожидаемого, разница фиксируется в комментарии документа Receipt.Comment.
    /// </remarks>
    /// <param name="id">Идентификатор документа.</param>
    /// <param name="request">Данные о фактически принятых товарах.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="204">Приёмка успешно подтверждена.</response>
    /// <response code="400">Ошибка валидации или бизнес-правила (например, ячейка занята).</response>
    /// <response code="404">Документ не найден.</response>
    [HttpPut("{id}/receive")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief + "," + Roles.Worker)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> ReceiveReceipt(
        int id,
        [FromBody] ReceiveReceiptRequest request,
        CancellationToken cancellationToken)
    {
        await receiveReceiptValidator.ValidateAndThrowAsync(request, cancellationToken);

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

    /// <summary>Отклонить приёмку (доступно Manager/Chief).</summary>
    /// <param name="id">Идентификатор документа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <response code="204">Приёмка отклонена.</response>
    [HttpPut("{id}/reject")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectReceipt(int id, CancellationToken cancellationToken)
    {
        await receiptService.RejectReceiptAsync(id, cancellationToken);
        return NoContent();
    }
}