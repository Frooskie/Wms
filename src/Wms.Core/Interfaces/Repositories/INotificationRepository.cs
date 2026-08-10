using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId,
        bool? isRead = null,
        CancellationToken cancellationToken = default);
}