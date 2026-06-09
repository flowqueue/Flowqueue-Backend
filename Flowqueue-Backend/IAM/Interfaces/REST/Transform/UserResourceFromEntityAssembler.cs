using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;

namespace Flowqueue_Backend.IAM.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user) =>
        new(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Value,
            user.DocumentNumber,
            user.CreatedAt,
            user.UpdatedAt);
}
