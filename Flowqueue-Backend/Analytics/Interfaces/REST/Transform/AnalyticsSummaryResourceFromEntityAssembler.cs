using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;
using Flowqueue_Backend.Analytics.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Analytics.Interfaces.REST.Transform;

public static class AnalyticsSummaryResourceFromEntityAssembler
{
    public static AnalyticsSummaryResource ToResourceFromEntity(AnalyticsSummary entity) =>
        new(
            entity.TotalTurns,
            entity.WaitingTurns,
            entity.CalledTurns,
            entity.CompletedTurns,
            entity.CancelledTurns,
            entity.AbsentTurns,
            entity.AverageWaitingMinutes,
            entity.AverageServiceMinutes);
}
