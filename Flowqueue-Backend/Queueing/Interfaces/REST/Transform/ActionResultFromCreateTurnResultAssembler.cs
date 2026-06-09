using Flowqueue_Backend.Queueing.Application.Errors;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Queueing.Interfaces.REST.Transform;

public static class ActionResultFromCreateTurnResultAssembler
{
    public static ActionResult<TurnResource> ToActionResultFromCreateTurnResult(
        Result<Turn, CreateTurnError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<Turn, CreateTurnError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                TurnResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Turn, CreateTurnError>.Failure failure => failure.Error switch
            {
                CreateTurnError.BranchOfficeNotFound => controller.NotFound("Branch office was not found."),
                CreateTurnError.ServiceNotFound => controller.NotFound("Service was not found."),
                CreateTurnError.ServiceDoesNotBelongToBranchOffice => controller.BadRequest("Service does not belong to the selected branch office."),
                _ => controller.Problem(statusCode: 500, detail: "Turn could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Turn could not be created.")
        };
}
