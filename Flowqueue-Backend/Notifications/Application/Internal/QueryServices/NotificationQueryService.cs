using Flowqueue_Backend.Notifications.Application.Services;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.Queries;
using Flowqueue_Backend.Notifications.Domain.Repositories;

namespace Flowqueue_Backend.Notifications.Application.Internal.QueryServices;

public class NotificationQueryService(INotificationRepository notificationRepository) : INotificationQueryService
{
    public async Task<IEnumerable<Notification>> Handle(
        GetAllNotificationsQuery query,
        CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByFiltersAsync(query.UserId, query.Status, query.Type, cancellationToken);

    public async Task<Notification?> Handle(
        GetNotificationByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByIdAsync(query.NotificationId, cancellationToken);

    public async Task<IEnumerable<Notification>> Handle(
        GetNotificationsByUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByUserIdAsync(query.UserId, cancellationToken);
}
