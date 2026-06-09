namespace Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;

public sealed record NotificationStatus
{
    public const string Unread = "unread";
    public const string Read = "read";
    public const string Archived = "archived";

    private static readonly string[] AllowedValues = [Unread, Read, Archived];

    public NotificationStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Notification status cannot be empty.", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        if (!AllowedValues.Contains(normalizedValue))
            throw new ArgumentException("Notification status is invalid.", nameof(value));

        Value = normalizedValue;
    }

    public string Value { get; }

    public static NotificationStatus NewUnread() => new(Unread);
    public static NotificationStatus NewRead() => new(Read);
    public static NotificationStatus NewArchived() => new(Archived);

    public override string ToString() => Value;
}
