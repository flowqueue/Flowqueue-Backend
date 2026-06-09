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
[Tags("Services")]
public class ServicesController(
    IServiceCommandService serviceCommandService,
    IServiceQueryService serviceQueryService,
    ILogger<ServicesController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a service", OperationId = "CreateService")]
    [SwaggerResponse(201, "Created", typeof(ServiceResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(409, "Conflict", typeof(string))]
    public async Task<ActionResult<ServiceResource>> CreateService(
        [FromBody] CreateServiceResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateServiceCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await serviceCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateServiceResultAssembler
                .ToActionResultFromCreateServiceResult(result, this, nameof(GetServiceById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid service request");
            return BadRequest("Invalid service request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets services", OperationId = "GetServices")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<ServiceResource>))]
    public async Task<ActionResult<IEnumerable<ServiceResource>>> GetServices(
        [FromQuery] int? branchOfficeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllServicesQuery(branchOfficeId);
        var result = await serviceQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(ServiceResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a service by id", OperationId = "GetServiceById")]
    [SwaggerResponse(200, "OK", typeof(ServiceResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<ServiceResource>> GetServiceById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetServiceByIdQuery(id);
        var result = await serviceQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(ServiceResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
