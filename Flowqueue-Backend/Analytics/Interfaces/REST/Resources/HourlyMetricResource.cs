namespace Flowqueue_Backend.Analytics.Interfaces.REST.Resources;

public record HourlyMetricResource(
    DateTimeOffset PeriodStart,
    int BranchOfficeId,
    int? ServiceId,
    int TotalTurns,
    int CompletedTurns,
    int CancelledTurns,
    decimal AverageWaitingMinutes);
