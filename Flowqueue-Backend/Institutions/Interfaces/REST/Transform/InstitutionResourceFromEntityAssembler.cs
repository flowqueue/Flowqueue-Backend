using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class InstitutionResourceFromEntityAssembler
{
    public static InstitutionResource ToResourceFromEntity(Institution entity) =>
        new(entity.Id, entity.Name, entity.Description, entity.Type.Value);
}
