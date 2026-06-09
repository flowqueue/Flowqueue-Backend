namespace Flowqueue_Backend.Notifications.Interfaces.REST.Resources;

public record NotificationResource(
    int Id,
    int UserId,
    int? TurnId,
    string Title,
    string Message,
    string Type,
    string Status,
    DateTimeOffset SentAt,
    DateTimeOffset? ReadAt,
    DateTimeOffset? ArchivedAt);
