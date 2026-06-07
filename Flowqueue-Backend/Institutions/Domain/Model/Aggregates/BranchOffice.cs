using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.Institutions.Domain.Model.Aggregates;

public partial class BranchOffice : IAuditableEntity
{
    protected BranchOffice()
    {
        Name = null!;
        Address = null!;
        District = null!;
        Schedule = null!;
    }

    public BranchOffice(CreateBranchOfficeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.InstitutionId <= 0)
            throw new ArgumentException("Institution id must be greater than zero.", nameof(command.InstitutionId));

        InstitutionId = command.InstitutionId;
        Name = NormalizeRequiredText(command.Name, nameof(command.Name));
        Address = NormalizeRequiredText(command.Address, nameof(command.Address));
        District = NormalizeRequiredText(command.District, nameof(command.District));
        Schedule = NormalizeRequiredText(command.Schedule, nameof(command.Schedule));
    }

    public int Id { get; private set; }
    public int InstitutionId { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string District { get; private set; }
    public string Schedule { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }
}
