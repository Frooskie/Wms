using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class NotificationRepository(ApplicationDbContext context)
    : Repository<Notification>(context), INotificationRepository
{
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId,
        bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(n => n.UserId == userId);
        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        query = query.OrderByDescending(n => n.CreatedAt);
        return await query.ToListAsync(cancellationToken);
    }
}