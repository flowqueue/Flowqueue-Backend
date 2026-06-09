using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.IAM.Interfaces.REST.Transform;

public static class ActionResultFromSignInUserResultAssembler
{
    public static ActionResult<UserResource> ToActionResultFromSignInUserResult(
        Result<User, SignInUserError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<User, SignInUserError>.Success success =>
                controller.Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<User, SignInUserError>.Failure failure => failure.Error switch
            {
                SignInUserError.InvalidCredentials => controller.Unauthorized("Invalid credentials."),
                _ => controller.Problem(statusCode: 500, detail: "Sign in could not be completed.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Sign in could not be completed.")
        };
}
