namespace Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

public record BranchOfficeResource(
    int Id,
    int InstitutionId,
    string Name,
    string Address,
    string District,
    string Schedule);
