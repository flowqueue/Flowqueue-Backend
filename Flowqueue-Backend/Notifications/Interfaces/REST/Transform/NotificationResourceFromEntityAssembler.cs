using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Notifications.Interfaces.REST.Transform;

public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification notification) =>
        new(
            notification.Id,
            notification.UserId,
            notification.TurnId,
            notification.Title,
            notification.Message,
            notification.Type.Value,
            notification.Status.Value,
            notification.SentAt,
            notification.ReadAt,
            notification.ArchivedAt);
}
