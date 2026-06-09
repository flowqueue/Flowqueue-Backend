namespace Flowqueue_Backend.Queueing.Interfaces.REST.Resources;

public record CreateTurnResource(
    int BranchOfficeId,
    int ServiceId,
    string CitizenName,
    string CitizenDocumentNumber);
