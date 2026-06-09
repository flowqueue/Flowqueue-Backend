namespace Flowqueue_Backend.Notifications.Interfaces.REST.Resources;

public record CreateNotificationResource(
    int UserId,
    int? TurnId,
    string Title,
    string Message,
    string Type);
