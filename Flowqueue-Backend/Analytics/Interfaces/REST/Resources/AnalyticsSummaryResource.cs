namespace Flowqueue_Backend.Analytics.Interfaces.REST.Resources;

public record AnalyticsSummaryResource(
    int TotalTurns,
    int WaitingTurns,
    int CalledTurns,
    int CompletedTurns,
    int CancelledTurns,
    int AbsentTurns,
    decimal AverageWaitingMinutes,
    decimal AverageServiceMinutes);
