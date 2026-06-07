namespace Flowqueue_Backend.Institutions.Domain.Model.Commands;

public record CreateBranchOfficeCommand(
    int InstitutionId,
    string Name,
    string Address,
    string District,
    string Schedule);
