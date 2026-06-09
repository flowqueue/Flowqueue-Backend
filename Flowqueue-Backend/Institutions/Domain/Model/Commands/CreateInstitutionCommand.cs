using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;

namespace Flowqueue_Backend.Institutions.Domain.Model.Commands;

public record CreateInstitutionCommand(string Name, string Description, InstitutionType Type);
