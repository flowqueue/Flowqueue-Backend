namespace Flowqueue_Backend.Queueing.Domain.Model.Commands;

public record CreateTurnCommand(
    int BranchOfficeId,
    int ServiceId,
    string CitizenName,
    string CitizenDocumentNumber);
