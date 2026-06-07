namespace Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

public record CreateServiceResource(
    int BranchOfficeId,
    string Name,
    int AverageDurationMinutes,
    string Prefix);
