using Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;

namespace Flowqueue_Backend.Notifications.Domain.Model.Commands;

public record CreateNotificationCommand(
    int UserId,
    int? TurnId,
    string Title,
    string Message,
    NotificationType Type);
