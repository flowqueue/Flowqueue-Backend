namespace Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;

public sealed record ServicePrefix
{
    public string Value { get; }

    public ServicePrefix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Service prefix cannot be empty.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public override string ToString() => Value;
}
