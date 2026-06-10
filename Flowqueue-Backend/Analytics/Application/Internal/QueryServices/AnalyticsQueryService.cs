using Flowqueue_Backend.Analytics.Application.Services;
using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;
using Flowqueue_Backend.Analytics.Domain.Model.Queries;
using Flowqueue_Backend.Analytics.Domain.Model.ValueObjects;
using Flowqueue_Backend.Analytics.Domain.Repositories;

namespace Flowqueue_Backend.Analytics.Application.Internal.QueryServices;

public class AnalyticsQueryService(IAnalyticsRepository analyticsRepository) : IAnalyticsQueryService
{
    public async Task<AnalyticsSummary> Handle(
        GetAnalyticsSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        await analyticsRepository.GetSummaryAsync(query.BranchOfficeId, query.ServiceId, cancellationToken);

    public async Task<IEnumerable<HourlyMetric>> Handle(
        GetHourlyMetricsQuery query,
        CancellationToken cancellationToken = default)
    {
        var period = new AnalyticsPeriod(query.From, query.To);
        return await analyticsRepository.GetHourlyMetricsAsync(
            query.BranchOfficeId,
            query.ServiceId,
            period.From,
            period.To,
            cancellationToken);
    }
}
