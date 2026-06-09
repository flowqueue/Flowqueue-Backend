using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class ActionResultFromCreateServiceResultAssembler
{
    public static ActionResult<ServiceResource> ToActionResultFromCreateServiceResult(
        Result<Service, CreateServiceError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<Service, CreateServiceError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                ServiceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Service, CreateServiceError>.Failure failure => failure.Error switch
            {
                CreateServiceError.BranchOfficeNotFound => controller.NotFound("Branch office was not found."),
                CreateServiceError.DuplicateService => controller.Conflict("Service already exists."),
                _ => controller.Problem(statusCode: 500, detail: "Service could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Service could not be created.")
        };
}
