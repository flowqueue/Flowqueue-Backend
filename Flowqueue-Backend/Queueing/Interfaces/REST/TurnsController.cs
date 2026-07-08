using System.Net.Mime;
using Flowqueue_Backend.Queueing.Application.Errors;
using Flowqueue_Backend.Queueing.Application.Services;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.Commands;
using Flowqueue_Backend.Queueing.Domain.Model.Queries;
using Flowqueue_Backend.Queueing.Interfaces.REST.Resources;
using Flowqueue_Backend.Queueing.Interfaces.REST.Transform;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.Queueing.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Turns")]
public class TurnsController(
    ITurnCommandService turnCommandService,
    ITurnQueryService turnQueryService,
    ILogger<TurnsController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a turn", OperationId = "CreateTurn")]
    [SwaggerResponse(201, "Created", typeof(TurnResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<TurnResource>> CreateTurn(
        [FromBody] CreateTurnResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateTurnCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await turnCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateTurnResultAssembler
                .ToActionResultFromCreateTurnResult(result, this, nameof(GetTurnById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid turn request");
            return BadRequest("Invalid turn request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets turns", OperationId = "GetTurns")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<TurnResource>))]
    public async Task<ActionResult<IEnumerable<TurnResource>>> GetTurns(
        [FromQuery] int? branchOfficeId = null,
        [FromQuery] int? serviceId = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTurnsQuery(branchOfficeId, serviceId, status);
        var result = await turnQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(TurnResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a turn by id", OperationId = "GetTurnById")]
    [SwaggerResponse(200, "OK", typeof(TurnResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<TurnResource>> GetTurnById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTurnByIdQuery(id);
        var result = await turnQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(TurnResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPatch("{id}/call")]
    [SwaggerOperation(Summary = "Calls a turn", OperationId = "CallTurn")]
    [SwaggerResponse(200, "OK", typeof(TurnResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<TurnResource>> CallTurn(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await turnCommandService.Handle(new CallTurnCommand(id), cancellationToken);
        return ActionResultFromUpdateTurnResultAssembler.ToActionResultFromUpdateTurnResult(result, this);
    }

    [HttpPatch("{id}/complete")]
    [SwaggerOperation(Summary = "Completes a turn", OperationId = "CompleteTurn")]
    [SwaggerResponse(200, "OK", typeof(TurnResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<TurnResource>> CompleteTurn(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await turnCommandService.Handle(new CompleteTurnCommand(id), cancellationToken);
        return ActionResultFromUpdateTurnResultAssembler.ToActionResultFromUpdateTurnResult(result, this);
    }

    [HttpPatch("{id}/cancel")]
    [SwaggerOperation(Summary = "Cancels a turn", OperationId = "CancelTurn")]
    [SwaggerResponse(200, "OK", typeof(TurnResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<TurnResource>> CancelTurn(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await turnCommandService.Handle(new CancelTurnCommand(id), cancellationToken);
        return ActionResultFromUpdateTurnResultAssembler.ToActionResultFromUpdateTurnResult(result, this);
    }

    [HttpPatch("{id}/mark-as-absent")]
    [SwaggerOperation(Summary = "Marks a turn as absent", OperationId = "MarkTurnAsAbsent")]
    [SwaggerResponse(200, "OK", typeof(TurnResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<TurnResource>> MarkTurnAsAbsent(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await turnCommandService.Handle(new MarkTurnAsAbsentCommand(id), cancellationToken);
        return ActionResultFromUpdateTurnResultAssembler.ToActionResultFromUpdateTurnResult(result, this);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletes a turn", OperationId = "DeleteTurn")]
    [SwaggerResponse(204, "No Content")]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult> DeleteTurn(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await turnCommandService.Handle(new DeleteTurnCommand(id), cancellationToken);

        return result switch
        {
            Result<Turn, UpdateTurnError>.Success => NoContent(),
            Result<Turn, UpdateTurnError>.Failure { Error: UpdateTurnError.TurnNotFound } =>
                NotFound("Turn was not found."),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Turn could not be deleted.")
        };
    }
}
