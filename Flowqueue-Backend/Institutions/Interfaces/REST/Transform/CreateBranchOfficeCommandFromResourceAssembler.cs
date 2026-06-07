using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class CreateBranchOfficeCommandFromResourceAssembler
{
    public static CreateBranchOfficeCommand ToCommandFromResource(CreateBranchOfficeResource resource) =>
        new(resource.InstitutionId, resource.Name, resource.Address, resource.District, resource.Schedule);
}
