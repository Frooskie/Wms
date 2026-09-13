using Microsoft.AspNetCore.Identity;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Notifications;

namespace Wms.Services.Notifications;

public class NotificationService(
    INotificationRepository notificationRepository,
    UserManager<ApplicationUser> userManager)
    : INotificationService
{
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId,
        bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        return await notificationRepository.GetUserNotificationsAsync(userId, isRead, cancellationToken);
    }

    public async Task MarkAsReadAsync(int notificationId, string userId, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
            throw new NotFoundException(ErrorMessages.Notification.NotFoundFormat(notificationId));
        if (notification.UserId != userId)
            throw new ForbiddenAccessException(ErrorMessages.Notification.AccessDenied);

        notification.IsRead = true;
        notificationRepository.Update(notification);
        await notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int notificationId, string userId, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
            throw new NotFoundException(ErrorMessages.Notification.NotFoundFormat(notificationId));
        if (notification.UserId != userId)
            throw new ForbiddenAccessException(ErrorMessages.Notification.AccessDenied);

        notificationRepository.Delete(notification);
        await notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateNotificationAsync(
        string userId,
        string title,
        string message,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        await notificationRepository.AddAsync(notification, cancellationToken);
        await notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyManagersAsync(string title, string message, CancellationToken cancellationToken)
    {
        var managers = await userManager.GetUsersInRoleAsync("Manager");
        foreach (var manager in managers)
        {
            await CreateNotificationAsync(manager.Id, title, message, cancellationToken);
        }
    }
}