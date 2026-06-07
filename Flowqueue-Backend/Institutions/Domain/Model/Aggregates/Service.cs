using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.Institutions.Domain.Model.Aggregates;

public partial class Service : IAuditableEntity
{
    protected Service()
    {
        Name = null!;
        Prefix = null!;
    }

    public Service(CreateServiceCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.BranchOfficeId <= 0)
            throw new ArgumentException("Branch office id must be greater than zero.", nameof(command.BranchOfficeId));

        if (command.AverageDurationMinutes <= 0)
            throw new ArgumentException("Average duration must be greater than zero.", nameof(command.AverageDurationMinutes));

        BranchOfficeId = command.BranchOfficeId;
        Name = NormalizeRequiredText(command.Name, nameof(command.Name));
        AverageDurationMinutes = command.AverageDurationMinutes;
        Prefix = command.Prefix ?? throw new ArgumentNullException(nameof(command.Prefix));
    }

    public int Id { get; private set; }
    public int BranchOfficeId { get; private set; }
    public string Name { get; private set; }
    public int AverageDurationMinutes { get; private set; }
    public ServicePrefix Prefix { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }
}
