using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class ActionResultFromCreateInstitutionResultAssembler
{
    public static ActionResult<InstitutionResource> ToActionResultFromCreateInstitutionResult(
        Result<Institution, CreateInstitutionError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<Institution, CreateInstitutionError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                InstitutionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Institution, CreateInstitutionError>.Failure failure => failure.Error switch
            {
                CreateInstitutionError.DuplicateInstitution => controller.Conflict("Institution already exists."),
                _ => controller.Problem(statusCode: 500, detail: "Institution could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Institution could not be created.")
        };
}
