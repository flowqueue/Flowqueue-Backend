namespace Flowqueue_Backend.Analytics.Domain.Model.Queries;

public record GetAnalyticsSummaryQuery(int? BranchOfficeId = null, int? ServiceId = null);
