using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Notifications;
using Wms.Core.Interfaces.Services.Notifications;

namespace Wms.API.Controllers;

/// <summary>Управление уведомлениями пользователя.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>Получить уведомления текущего пользователя.</summary>
    [HttpGet]
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
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated");
        await notificationService.MarkAsReadAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>Удалить уведомление.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated");
        await notificationService.DeleteAsync(id, userId, cancellationToken);
        return NoContent();
    }
}