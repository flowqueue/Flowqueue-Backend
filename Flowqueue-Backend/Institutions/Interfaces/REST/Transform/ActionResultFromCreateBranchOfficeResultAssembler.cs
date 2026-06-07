using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Institutions.Interfaces.REST.Transform;

public static class ActionResultFromCreateBranchOfficeResultAssembler
{
    public static ActionResult<BranchOfficeResource> ToActionResultFromCreateBranchOfficeResult(
        Result<BranchOffice, CreateBranchOfficeError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<BranchOffice, CreateBranchOfficeError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                BranchOfficeResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<BranchOffice, CreateBranchOfficeError>.Failure failure => failure.Error switch
            {
                CreateBranchOfficeError.InstitutionNotFound => controller.NotFound("Institution was not found."),
                CreateBranchOfficeError.DuplicateBranchOffice => controller.Conflict("Branch office already exists."),
                _ => controller.Problem(statusCode: 500, detail: "Branch office could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Branch office could not be created.")
        };
}
