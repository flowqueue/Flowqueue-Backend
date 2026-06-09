using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class ServiceResourceFromEntityAssembler
{
    public static ServiceResource ToResourceFromEntity(Service entity) =>
        new(entity.Id, entity.BranchOfficeId, entity.Name, entity.AverageDurationMinutes, entity.Prefix.Value);
}
