namespace Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;

public sealed record TurnStatus
{
    private static readonly string[] AllowedValues = [Waiting, Called, Completed, Cancelled, Absent];

    public const string Waiting = "waiting";
    public const string Called = "called";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";
    public const string Absent = "absent";

    public string Value { get; }

    public TurnStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Turn status cannot be empty.", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        if (!AllowedValues.Contains(normalizedValue))
            throw new ArgumentException("Invalid turn status.", nameof(value));

        Value = normalizedValue;
    }

    public static TurnStatus NewWaiting() => new(Waiting);
    public static TurnStatus NewCalled() => new(Called);
    public static TurnStatus NewCompleted() => new(Completed);
    public static TurnStatus NewCancelled() => new(Cancelled);
    public static TurnStatus NewAbsent() => new(Absent);
    public override string ToString() => Value;
}
