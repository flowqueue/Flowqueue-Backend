using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;
using Flowqueue_Backend.Notifications.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Notifications.Infrastructure.Persistence.EFC.Repositories;

public class NotificationRepository(AppDbContext context) : BaseRepository<Notification>(context), INotificationRepository
{
    public new async Task AddAsync(Notification notification, CancellationToken cancellationToken = default) =>
        await Context.Set<Notification>().AddAsync(notification, cancellationToken);

    public async Task<IEnumerable<Notification>> FindByFiltersAsync(
        int? userId,
        string? status,
        string? type,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Notification>().AsQueryable();

        if (userId.HasValue)
            query = query.Where(notification => notification.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            NotificationStatus normalizedStatus;
            try
            {
                normalizedStatus = new NotificationStatus(status);
            }
            catch (ArgumentException)
            {
                return [];
            }

            query = query.Where(notification => notification.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            NotificationType normalizedType;
            try
            {
                normalizedType = new NotificationType(type);
            }
            catch (ArgumentException)
            {
                return [];
            }

            query = query.Where(notification => notification.Type == normalizedType);
        }

        return await query
            .OrderByDescending(notification => notification.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> FindByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Notification>()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.SentAt)
            .ToListAsync(cancellationToken);
}
