using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.IAM.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    protected User()
    {
        FullName = null!;
        Email = null!;
        PasswordHash = null!;
        Role = null!;
    }

    public User(CreateUserCommand command, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(command);

        FullName = NormalizeRequiredText(command.FullName, nameof(command.FullName));
        Email = NormalizeEmail(command.Email);
        PasswordHash = NormalizeRequiredText(passwordHash, nameof(passwordHash));
        Role = command.Role ?? throw new ArgumentNullException(nameof(command.Role));
        DocumentNumber = NormalizeOptionalText(command.DocumentNumber);
    }

    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public string? DocumentNumber { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public bool HasPasswordHash(string passwordHash) => PasswordHash == passwordHash;

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = NormalizeRequiredText(passwordHash, nameof(passwordHash));
    }

    public static string NormalizeEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        var normalizedEmail = value.Trim().ToLowerInvariant();
        if (!normalizedEmail.Contains('@') || normalizedEmail.Length < 5)
            throw new ArgumentException("Email is invalid.", nameof(value));

        return normalizedEmail;
    }

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
