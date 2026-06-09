namespace Flowqueue_Backend.IAM.Domain.Model.ValueObjects;

public sealed record UserRole
{
    public const string Citizen = "citizen";
    public const string Operator = "operator";
    public const string Supervisor = "supervisor";
    public const string Admin = "admin";

    private static readonly HashSet<string> AllowedRoles =
    [
        Citizen,
        Operator,
        Supervisor,
        Admin
    ];

    public UserRole(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User role cannot be empty.", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        if (!AllowedRoles.Contains(normalizedValue))
            throw new ArgumentException("Invalid user role.", nameof(value));

        Value = normalizedValue;
    }

    public string Value { get; }

    public static UserRole NewCitizen() => new(Citizen);
    public static UserRole NewOperator() => new(Operator);
    public static UserRole NewSupervisor() => new(Supervisor);
    public static UserRole NewAdmin() => new(Admin);
}
