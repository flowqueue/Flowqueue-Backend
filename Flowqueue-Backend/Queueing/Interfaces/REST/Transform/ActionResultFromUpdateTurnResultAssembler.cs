using Flowqueue_Backend.Queueing.Application.Errors;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Queueing.Interfaces.REST.Transform;

public static class ActionResultFromUpdateTurnResultAssembler
{
    public static ActionResult<TurnResource> ToActionResultFromUpdateTurnResult(
        Result<Turn, UpdateTurnError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<Turn, UpdateTurnError>.Success success =>
                controller.Ok(TurnResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Turn, UpdateTurnError>.Failure failure => failure.Error switch
            {
                UpdateTurnError.TurnNotFound => controller.NotFound("Turn was not found."),
                UpdateTurnError.InvalidStatusChange => controller.BadRequest("Turn status cannot be changed with the requested operation."),
                _ => controller.Problem(statusCode: 500, detail: "Turn could not be updated.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Turn could not be updated.")
        };
}
