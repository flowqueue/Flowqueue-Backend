namespace Flowqueue_Backend.Queueing.Domain.Model.Queries;

public record GetAllTurnsQuery(int? BranchOfficeId, int? ServiceId, string? Status);
