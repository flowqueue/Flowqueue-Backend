using System.Net.Mime;
using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;
using Flowqueue_Backend.IAM.Interfaces.REST.Transform;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/auth")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IAuthenticationCommandService authenticationCommandService,
    ITokenService tokenService,
    ILogger<AuthenticationController> logger) : ControllerBase
{
    private static readonly HashSet<string> PublicSignUpRoles =
    [
        UserRole.Citizen,
        UserRole.Operator
    ];

    [AllowAnonymous]
    [HttpPost("sign-up")]
    [SwaggerOperation(Summary = "Signs up a user", OperationId = "SignUp")]
    [SwaggerResponse(201, "Created", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(409, "Conflict", typeof(string))]
    public async Task<ActionResult<AuthenticatedUserResource>> SignUp(
        [FromBody] CreateUserResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!IsPublicSignUpRole(resource.Role))
                return BadRequest("Public sign up only supports citizen or operator roles.");

            var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await userCommandService.Handle(command, cancellationToken);
            return ToSignUpActionResult(result);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid sign up request");
            return BadRequest("Invalid sign up request.");
        }
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    [SwaggerOperation(Summary = "Signs in a user", OperationId = "SignIn")]
    [SwaggerResponse(200, "OK", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(401, "Unauthorized", typeof(string))]
    public async Task<ActionResult<AuthenticatedUserResource>> SignIn(
        [FromBody] SignInUserResource resource,
        CancellationToken cancellationToken)
    {
        var command = SignInUserCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await authenticationCommandService.Handle(command, cancellationToken);
        return ToSignInActionResult(result);
    }

    private ActionResult<AuthenticatedUserResource> ToSignUpActionResult(Result<User, CreateUserError> result) =>
        result switch
        {
            Result<User, CreateUserError>.Success success => Created(
                $"/api/v1/users/{success.Value.Id}",
                ToAuthenticatedResource(success.Value)),
            Result<User, CreateUserError>.Failure failure => failure.Error switch
            {
                CreateUserError.DuplicateEmail => Conflict("User email already exists."),
                CreateUserError.InvalidUserData => BadRequest("Invalid sign up request."),
                _ => Problem(statusCode: 500, detail: "Sign up could not be completed.")
            },
            _ => Problem(statusCode: 500, detail: "Sign up could not be completed.")
        };

    private ActionResult<AuthenticatedUserResource> ToSignInActionResult(Result<User, SignInUserError> result) =>
        result switch
        {
            Result<User, SignInUserError>.Success success => Ok(ToAuthenticatedResource(success.Value)),
            Result<User, SignInUserError>.Failure failure => failure.Error switch
            {
                SignInUserError.InvalidCredentials => Unauthorized("Invalid credentials."),
                _ => Problem(statusCode: 500, detail: "Sign in could not be completed.")
            },
            _ => Problem(statusCode: 500, detail: "Sign in could not be completed.")
        };

    private AuthenticatedUserResource ToAuthenticatedResource(User user) =>
        new(UserResourceFromEntityAssembler.ToResourceFromEntity(user), tokenService.GenerateToken(user));

    private static bool IsPublicSignUpRole(string role) =>
        !string.IsNullOrWhiteSpace(role)
        && PublicSignUpRoles.Contains(role.Trim().ToLowerInvariant());
}
