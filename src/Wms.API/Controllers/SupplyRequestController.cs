using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Supply;
using Wms.API.Extensions;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Documents;

namespace Wms.API.Controllers;

/// <summary>Управление заявками магазина на поставку.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SupplyRequestController(
    ISupplyRequestService service,
    IMapper mapper,
    IValidator<CreateSupplyRequestRequest> createSupplyRequestValidator)
    : ControllerBase
{
    /// <summary>Создать новую заявку на поставку.</summary>
    /// <param name="request">Данные заявки.</param>
    /// <response code="201">Заявка создана.</response>
    /// <response code="400">Ошибка валидации.</response>
    /// <response code="401">Не авторизован.</response>
    /// <response code="403">Недостаточно прав (только StoreDirector).</response>
    [HttpPost]
    [Authorize(Roles = Roles.StoreDirector)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SupplyRequestDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SupplyRequestDto>> Create([FromBody] CreateSupplyRequestRequest request,
        CancellationToken cancellationToken)
    {
        await createSupplyRequestValidator.ValidateAndThrowAsync(request, cancellationToken);
        
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

    /// <summary>Получить список заявок с фильтрацией по статусу и создателю.</summary>
    /// <param name="status">Статус (необязательно).</param>
    /// <param name="createdBy">Создатель (необязательно).</param>
    /// <response code="200">Список заявок.</response>
    /// <response code="400">Неверное значение параметра status.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SupplyRequestDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<IEnumerable<SupplyRequestDto>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? createdBy,
        CancellationToken cancellationToken)
    {
        var requests = await service.GetAllWithLinesAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(status))
        {
            if (!Enum.TryParse<SupplyRequestStatus>(status, true, out var statusEnum))
                throw new BusinessRuleException(
                    $"Недопустимое значение статуса '{status}'. Допустимые значения: {string.Join(", ", Enum.GetNames<SupplyRequestStatus>())}.",
                    "INVALID_STATUS");
            
            requests = requests.Where(r => r.Status == statusEnum);
        }
        
        if (!string.IsNullOrEmpty(createdBy))
            requests = requests.Where(r => r.CreatedBy == createdBy);
        
        return Ok(mapper.Map<IEnumerable<SupplyRequestDto>>(requests));
    }

    /// <summary>Получить заявку по идентификатору.</summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <response code="200">Заявка найдена.</response>
    /// <response code="404">Заявка не найдена.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplyRequestDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<SupplyRequestDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var req = await service.GetByIdWithLinesAsync(id, cancellationToken);
        if (req == null)
            throw new NotFoundException(ErrorMessages.SupplyRequest.NotFoundFormat(id));
        
        return Ok(mapper.Map<SupplyRequestDto>(req));
    }

    /// <summary>Отправить заявку на рассмотрение. Доступно только создателю заявки.</summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <response code="204">Заявка отправлена.</response>
    /// <response code="400">Заявка не в статусе Draft.</response>
    /// <response code="403">Пользователь не является создателем заявки.</response>
    /// <response code="404">Заявка не найдена.</response>
    [HttpPut("{id}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Submit(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await service.SubmitAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>Одобрить заявку (доступно Manager/Chief).</summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <response code="204">Заявка одобрена.</response>
    /// <response code="400">Ошибка бизнес-правила.</response>
    /// <response code="401">Не авторизован.</response>
    /// <response code="403">Недостаточно прав.</response>
    /// <response code="404">Заявка не найдена.</response>
    [HttpPut("{id}/approve")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        await service.ApproveAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Отклонить заявку (доступно Manager/Chief).</summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <response code="204">Заявка отклонена.</response>
    /// <response code="400">Ошибка бизнес-правила.</response>
    /// <response code="401">Не авторизован.</response>
    /// <response code="403">Недостаточно прав.</response>
    /// <response code="404">Заявка не найдена.</response>
    [HttpPut("{id}/reject")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        await service.RejectAsync(id, cancellationToken);
        return NoContent();
    }
}