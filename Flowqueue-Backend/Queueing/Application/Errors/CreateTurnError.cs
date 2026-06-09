namespace Flowqueue_Backend.Queueing.Application.Errors;

public enum CreateTurnError
{
    BranchOfficeNotFound,
    ServiceNotFound,
    ServiceDoesNotBelongToBranchOffice,
    UnexpectedError
}
