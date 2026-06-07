using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class BranchOfficeResourceFromEntityAssembler
{
    public static BranchOfficeResource ToResourceFromEntity(BranchOffice entity) =>
        new(entity.Id, entity.InstitutionId, entity.Name, entity.Address, entity.District, entity.Schedule);
}
