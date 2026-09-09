using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Notifications;
using Wms.Core.Interfaces.Services.Notifications;

namespace Wms.API.Controllers;

/// <summary>Управление уведомлениями пользователя.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>Получить уведомления текущего пользователя.</summary>
    /// <response code="200">Список уведомлений.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificationResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetMyNotifications(
        [FromQuery] bool? isRead,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated");
        var notifications = await notificationService.GetUserNotificationsAsync(userId, isRead, cancellationToken);

        return Ok(notifications.Select(n => new NotificationResponseDto(
            n.Id,
            n.Title,
            n.Message,
            n.IsRead,
            n.CreatedAt)));
    }

    /// <summary>Отметить уведомление как прочитанное.</summary>
    /// <param name="id">Идентификатор уведомления.</param>
    /// <response code="204">Уведомление отмечено как прочитанное.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Уведомление не найдено или не принадлежит пользователю.</response>
    [HttpPut("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated");
        await notificationService.MarkAsReadAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>Удалить уведомление.</summary>
    /// <param name="id">Идентификатор уведомления.</param>
    /// <response code="204">Уведомление удалено.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Уведомление не найдено или не принадлежит пользователю.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated");
        await notificationService.DeleteAsync(id, userId, cancellationToken);
        return NoContent();
    }
}