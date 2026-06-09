using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.Queries;

namespace Flowqueue_Backend.Notifications.Application.Services;

public interface INotificationQueryService
{
    Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery query, CancellationToken cancellationToken = default);
    Task<Notification?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> Handle(GetNotificationsByUserIdQuery query, CancellationToken cancellationToken = default);
}
