using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;

namespace Flowqueue_Backend.Analytics.Domain.Repositories;

public interface IAnalyticsRepository
{
    Task<AnalyticsSummary> GetSummaryAsync(
        int? branchOfficeId = null,
        int? serviceId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HourlyMetric>> GetHourlyMetricsAsync(
        int? branchOfficeId = null,
        int? serviceId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);
}
