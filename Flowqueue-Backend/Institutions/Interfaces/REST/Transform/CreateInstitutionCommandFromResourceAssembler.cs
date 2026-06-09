using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class CreateInstitutionCommandFromResourceAssembler
{
    public static CreateInstitutionCommand ToCommandFromResource(CreateInstitutionResource resource) =>
        new(resource.Name, resource.Description, new InstitutionType(resource.Type));
}
