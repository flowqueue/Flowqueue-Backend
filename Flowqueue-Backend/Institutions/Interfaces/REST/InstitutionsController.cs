using System.Net.Mime;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;
using Flowqueue_Backend.Institutions.Interfaces.REST.Resources;
using Flowqueue_Backend.Institutions.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.Institutions.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Institutions")]
public class InstitutionsController(
    IInstitutionCommandService institutionCommandService,
    IInstitutionQueryService institutionQueryService,
    ILogger<InstitutionsController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates an institution", OperationId = "CreateInstitution")]
    [SwaggerResponse(201, "Created", typeof(InstitutionResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(409, "Conflict", typeof(string))]
    public async Task<ActionResult<InstitutionResource>> CreateInstitution(
        [FromBody] CreateInstitutionResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateInstitutionCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await institutionCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateInstitutionResultAssembler
                .ToActionResultFromCreateInstitutionResult(result, this, nameof(GetInstitutionById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid institution request");
            return BadRequest("Invalid institution request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets institutions", OperationId = "GetInstitutions")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<InstitutionResource>))]
    public async Task<ActionResult<IEnumerable<InstitutionResource>>> GetInstitutions(
        [FromQuery] string? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllInstitutionsQuery(type);
        var result = await institutionQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(InstitutionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets an institution by id", OperationId = "GetInstitutionById")]
    [SwaggerResponse(200, "OK", typeof(InstitutionResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<InstitutionResource>> GetInstitutionById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInstitutionByIdQuery(id);
        var result = await institutionQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(InstitutionResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
