namespace Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

public record ServiceResource(
    int Id,
    int BranchOfficeId,
    string Name,
    int AverageDurationMinutes,
    string Prefix);
