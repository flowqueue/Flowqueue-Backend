namespace Flowqueue_Backend.Analytics.Domain.Model.Queries;

public record GetHourlyMetricsQuery(
    int? BranchOfficeId = null,
    int? ServiceId = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null);
