namespace Flowqueue_Backend.Notifications.Domain.Model.Queries;

public record GetAllNotificationsQuery(int? UserId = null, string? Status = null, string? Type = null);
