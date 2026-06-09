using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;
using Flowqueue_Backend.Notifications.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Notifications.Interfaces.REST.Transform;

public static class CreateNotificationCommandFromResourceAssembler
{
    public static CreateNotificationCommand ToCommandFromResource(CreateNotificationResource resource) =>
        new(
            resource.UserId,
            resource.TurnId,
            resource.Title,
            resource.Message,
            new NotificationType(resource.Type));
}
