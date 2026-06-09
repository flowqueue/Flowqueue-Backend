using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.Notifications.Domain.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> FindByFiltersAsync(
        int? userId,
        string? status,
        string? type,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
