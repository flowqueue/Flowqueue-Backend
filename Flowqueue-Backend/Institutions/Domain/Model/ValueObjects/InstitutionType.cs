namespace Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;

public sealed record InstitutionType
{
    public string Value { get; }

    public InstitutionType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Institution type cannot be empty.", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
