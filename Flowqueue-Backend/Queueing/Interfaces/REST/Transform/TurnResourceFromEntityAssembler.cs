using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Queueing.Interfaces.REST.Transform;

public static class TurnResourceFromEntityAssembler
{
    public static TurnResource ToResourceFromEntity(Turn entity) =>
        new(
            entity.Id,
            entity.BranchOfficeId,
            entity.ServiceId,
            entity.CitizenName,
            entity.CitizenDocumentNumber,
            entity.TurnNumber,
            entity.TicketCode.Value,
            entity.Status.Value,
            entity.RegisteredAt,
            entity.CalledAt,
            entity.CompletedAt,
            entity.CancelledAt,
            entity.MarkedAbsentAt);
}
