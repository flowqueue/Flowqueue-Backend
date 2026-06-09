namespace Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;

public sealed record TicketCode
{
    public string Value { get; }

    public TicketCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Ticket code cannot be empty.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public override string ToString() => Value;
}
