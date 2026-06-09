using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.Notifications.Domain.Model.Aggregates;

public partial class Notification : IAuditableEntity
{
    protected Notification()
    {
        Title = null!;
        Message = null!;
        Type = null!;
        Status = null!;
    }

    public Notification(CreateNotificationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UserId <= 0)
            throw new ArgumentException("User id must be greater than zero.", nameof(command.UserId));

        if (command.TurnId is <= 0)
            throw new ArgumentException("Turn id must be greater than zero.", nameof(command.TurnId));

        UserId = command.UserId;
        TurnId = command.TurnId;
        Title = NormalizeRequiredText(command.Title, nameof(command.Title));
        Message = NormalizeRequiredText(command.Message, nameof(command.Message));
        Type = command.Type ?? throw new ArgumentNullException(nameof(command.Type));
        Status = NotificationStatus.NewUnread();
        SentAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int? TurnId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void MarkAsRead()
    {
        if (HasStatus(NotificationStatus.Archived))
            throw new InvalidOperationException("Archived notifications cannot be marked as read.");

        if (HasStatus(NotificationStatus.Read))
            return;

        Status = NotificationStatus.NewRead();
        ReadAt = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        if (HasStatus(NotificationStatus.Archived))
            return;

        Status = NotificationStatus.NewArchived();
        ArchivedAt = DateTimeOffset.UtcNow;
    }

    private bool HasStatus(string status) => Status.Value == status;

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }
}
