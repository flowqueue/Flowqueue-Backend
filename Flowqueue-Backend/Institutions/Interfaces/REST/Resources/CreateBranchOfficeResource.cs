namespace Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

public record CreateBranchOfficeResource(
    int InstitutionId,
    string Name,
    string Address,
    string District,
    string Schedule);
