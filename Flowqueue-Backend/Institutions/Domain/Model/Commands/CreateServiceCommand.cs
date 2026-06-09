using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;

namespace Flowqueue_Backend.Institutions.Domain.Model.Commands;

public record CreateServiceCommand(
    int BranchOfficeId,
    string Name,
    int AverageDurationMinutes,
    ServicePrefix Prefix);
