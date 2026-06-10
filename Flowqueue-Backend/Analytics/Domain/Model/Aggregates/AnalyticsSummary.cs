namespace Flowqueue_Backend.Analytics.Domain.Model.Aggregates;

public record AnalyticsSummary(
    int TotalTurns,
    int WaitingTurns,
    int CalledTurns,
    int CompletedTurns,
    int CancelledTurns,
    int AbsentTurns,
    decimal AverageWaitingMinutes,
    decimal AverageServiceMinutes);
