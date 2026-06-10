using System.Net.Mime;
using Flowqueue_Backend.Analytics.Application.Services;
using Flowqueue_Backend.Analytics.Domain.Model.Queries;
using Flowqueue_Backend.Analytics.Interfaces.REST.Resources;
using Flowqueue_Backend.Analytics.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.Analytics.Interfaces.REST;

[ApiController]
[Route("api/v1/analytics")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Analytics")]
public class AnalyticsController(IAnalyticsQueryService analyticsQueryService, ILogger<AnalyticsController> logger) : ControllerBase
{
    [HttpGet("summary")]
    [SwaggerOperation(Summary = "Gets analytics summary", OperationId = "GetAnalyticsSummary")]
    [SwaggerResponse(200, "OK", typeof(AnalyticsSummaryResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    public async Task<ActionResult<AnalyticsSummaryResource>> GetSummary(
        [FromQuery] int? branchOfficeId = null,
        [FromQuery] int? serviceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnalyticsSummaryQuery(branchOfficeId, serviceId);
        var result = await analyticsQueryService.Handle(query, cancellationToken);
        return Ok(AnalyticsSummaryResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpGet("branch-offices/{branchOfficeId:int}/summary")]
    [SwaggerOperation(Summary = "Gets analytics summary by branch office", OperationId = "GetBranchOfficeAnalyticsSummary")]
    [SwaggerResponse(200, "OK", typeof(AnalyticsSummaryResource))]
    public async Task<ActionResult<AnalyticsSummaryResource>> GetBranchOfficeSummary(
        int branchOfficeId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnalyticsSummaryQuery(branchOfficeId, null);
        var result = await analyticsQueryService.Handle(query, cancellationToken);
        return Ok(AnalyticsSummaryResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpGet("services/{serviceId:int}/summary")]
    [SwaggerOperation(Summary = "Gets analytics summary by service", OperationId = "GetServiceAnalyticsSummary")]
    [SwaggerResponse(200, "OK", typeof(AnalyticsSummaryResource))]
    public async Task<ActionResult<AnalyticsSummaryResource>> GetServiceSummary(
        int serviceId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnalyticsSummaryQuery(null, serviceId);
        var result = await analyticsQueryService.Handle(query, cancellationToken);
        return Ok(AnalyticsSummaryResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpGet("hourly")]
    [SwaggerOperation(Summary = "Gets hourly analytics metrics", OperationId = "GetHourlyAnalyticsMetrics")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<HourlyMetricResource>))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    public async Task<ActionResult<IEnumerable<HourlyMetricResource>>> GetHourlyMetrics(
        [FromQuery] int? branchOfficeId = null,
        [FromQuery] int? serviceId = null,
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetHourlyMetricsQuery(branchOfficeId, serviceId, from, to);
            var result = await analyticsQueryService.Handle(query, cancellationToken);
            return Ok(result.Select(HourlyMetricResourceFromEntityAssembler.ToResourceFromEntity));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid analytics date range");
            return BadRequest("Invalid analytics date range.");
        }
    }
}
