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
[Tags("Branch Offices")]
public class BranchOfficesController(
    IBranchOfficeCommandService branchOfficeCommandService,
    IBranchOfficeQueryService branchOfficeQueryService,
    ILogger<BranchOfficesController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a branch office", OperationId = "CreateBranchOffice")]
    [SwaggerResponse(201, "Created", typeof(BranchOfficeResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(409, "Conflict", typeof(string))]
    public async Task<ActionResult<BranchOfficeResource>> CreateBranchOffice(
        [FromBody] CreateBranchOfficeResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateBranchOfficeCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await branchOfficeCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateBranchOfficeResultAssembler
                .ToActionResultFromCreateBranchOfficeResult(result, this, nameof(GetBranchOfficeById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid branch office request");
            return BadRequest("Invalid branch office request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets branch offices", OperationId = "GetBranchOffices")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<BranchOfficeResource>))]
    public async Task<ActionResult<IEnumerable<BranchOfficeResource>>> GetBranchOffices(
        [FromQuery] int? institutionId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBranchOfficesQuery(institutionId);
        var result = await branchOfficeQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(BranchOfficeResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a branch office by id", OperationId = "GetBranchOfficeById")]
    [SwaggerResponse(200, "OK", typeof(BranchOfficeResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<BranchOfficeResource>> GetBranchOfficeById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBranchOfficeByIdQuery(id);
        var result = await branchOfficeQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(BranchOfficeResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
