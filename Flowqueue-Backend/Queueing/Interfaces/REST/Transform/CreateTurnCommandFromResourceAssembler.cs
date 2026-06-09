using Flowqueue_Backend.Queueing.Domain.Model.Commands;
using Flowqueue_Backend.Queueing.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Queueing.Interfaces.REST.Transform;

public static class CreateTurnCommandFromResourceAssembler
{
    public static CreateTurnCommand ToCommandFromResource(CreateTurnResource resource) =>
        new(resource.BranchOfficeId, resource.ServiceId, resource.CitizenName, resource.CitizenDocumentNumber);
}
