namespace Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;

public sealed record NotificationType
{
    public const string TurnCalled = "turn-called";
    public const string TurnNear = "turn-near";
    public const string TurnCancelled = "turn-cancelled";
    public const string TurnCompleted = "turn-completed";
    public const string General = "general";

    private static readonly string[] AllowedValues =
    [
        TurnCalled,
        TurnNear,
        TurnCancelled,
        TurnCompleted,
        General
    ];

    public NotificationType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Notification type cannot be empty.", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        if (!AllowedValues.Contains(normalizedValue))
            throw new ArgumentException("Notification type is invalid.", nameof(value));

        Value = normalizedValue;
    }

    public string Value { get; }

    public static NotificationType NewGeneral() => new(General);
    public static NotificationType NewTurnCalled() => new(TurnCalled);
    public static NotificationType NewTurnNear() => new(TurnNear);
    public static NotificationType NewTurnCancelled() => new(TurnCancelled);
    public static NotificationType NewTurnCompleted() => new(TurnCompleted);

    public override string ToString() => Value;
}
