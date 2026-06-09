using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.IAM.Interfaces.REST.Transform;

public static class ActionResultFromCreateUserResultAssembler
{
    public static ActionResult<UserResource> ToActionResultFromCreateUserResult(
        Result<User, CreateUserError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<User, CreateUserError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<User, CreateUserError>.Failure failure => failure.Error switch
            {
                CreateUserError.DuplicateEmail => controller.Conflict("User email already exists."),
                CreateUserError.InvalidUserData => controller.BadRequest("Invalid user request."),
                _ => controller.Problem(statusCode: 500, detail: "User could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "User could not be created.")
        };
}
