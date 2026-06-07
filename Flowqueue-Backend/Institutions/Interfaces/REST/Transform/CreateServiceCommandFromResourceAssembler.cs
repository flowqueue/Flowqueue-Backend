using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class CreateServiceCommandFromResourceAssembler
{
    public static CreateServiceCommand ToCommandFromResource(CreateServiceResource resource) =>
        new(resource.BranchOfficeId, resource.Name, resource.AverageDurationMinutes, new ServicePrefix(resource.Prefix));
}
