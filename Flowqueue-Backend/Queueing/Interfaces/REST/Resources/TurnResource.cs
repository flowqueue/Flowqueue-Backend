namespace Flowqueue_Backend.Queueing.Interfaces.REST.Resources;

public record TurnResource(
    int Id,
    int BranchOfficeId,
    int ServiceId,
    string CitizenName,
    string CitizenDocumentNumber,
    int TurnNumber,
    string TicketCode,
    string Status,
    DateTimeOffset RegisteredAt,
    DateTimeOffset? CalledAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? CancelledAt,
    DateTimeOffset? MarkedAbsentAt);
