using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services.Notifications;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId,
        bool? isRead = null,
        CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(int notificationId, string userId, CancellationToken cancellationToken);
    Task DeleteAsync(int notificationId, string userId, CancellationToken cancellationToken);

    Task CreateNotificationAsync(
        string userId,
        string title,
        string message,
        CancellationToken cancellationToken);

    Task NotifyManagersAsync(
        string title,
        string message,
        CancellationToken cancellationToken);
}