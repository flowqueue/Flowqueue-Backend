namespace Flowqueue_Backend.Analytics.Domain.Model.Aggregates;

public record HourlyMetric(
    DateTimeOffset PeriodStart,
    int BranchOfficeId,
    int? ServiceId,
    int TotalTurns,
    int CompletedTurns,
    int CancelledTurns,
    decimal AverageWaitingMinutes);
