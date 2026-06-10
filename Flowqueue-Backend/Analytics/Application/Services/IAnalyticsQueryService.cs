using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;
using Flowqueue_Backend.Analytics.Domain.Model.Queries;

namespace Flowqueue_Backend.Analytics.Application.Services;

public interface IAnalyticsQueryService
{
    Task<AnalyticsSummary> Handle(
        GetAnalyticsSummaryQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HourlyMetric>> Handle(
        GetHourlyMetricsQuery query,
        CancellationToken cancellationToken = default);
}
