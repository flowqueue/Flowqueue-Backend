using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;

namespace Flowqueue_Backend.IAM.Interfaces.REST.Transform;

public static class SignInUserCommandFromResourceAssembler
{
    public static SignInUserCommand ToCommandFromResource(SignInUserResource resource) =>
        new(resource.Email, resource.Password);
}
