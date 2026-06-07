using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.Institutions.Domain.Model.Aggregates;

public partial class Institution : IAuditableEntity
{
    protected Institution()
    {
        Name = null!;
        Description = null!;
        Type = null!;
    }

    public Institution(CreateInstitutionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        Name = NormalizeRequiredText(command.Name, nameof(command.Name));
        Description = NormalizeRequiredText(command.Description, nameof(command.Description));
        Type = command.Type ?? throw new ArgumentNullException(nameof(command.Type));
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public InstitutionType Type { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }
}
