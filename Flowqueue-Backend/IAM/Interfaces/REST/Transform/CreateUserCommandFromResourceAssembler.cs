using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;

namespace Flowqueue_Backend.IAM.Interfaces.REST.Transform;

public static class CreateUserCommandFromResourceAssembler
{
    public static CreateUserCommand ToCommandFromResource(CreateUserResource resource) =>
        new(
            resource.FullName,
            resource.Email,
            resource.Password,
            new UserRole(resource.Role),
            resource.DocumentNumber);
}
