using System.Net.Mime;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Queries;
using Flowqueue_Backend.IAM.Interfaces.REST.Resources;
using Flowqueue_Backend.IAM.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    ILogger<UsersController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a user", OperationId = "CreateUser")]
    [SwaggerResponse(201, "Created", typeof(UserResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(409, "Conflict", typeof(string))]
    public async Task<ActionResult<UserResource>> CreateUser(
        [FromBody] CreateUserResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await userCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateUserResultAssembler
                .ToActionResultFromCreateUserResult(result, this, nameof(GetUserById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid user request");
            return BadRequest("Invalid user request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets users", OperationId = "GetUsers")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<UserResource>))]
    public async Task<ActionResult<IEnumerable<UserResource>>> GetUsers(
        [FromQuery] string? role = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery(role);
        var result = await userQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a user by id", OperationId = "GetUserById")]
    [SwaggerResponse(200, "OK", typeof(UserResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<UserResource>> GetUserById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        var result = await userQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
