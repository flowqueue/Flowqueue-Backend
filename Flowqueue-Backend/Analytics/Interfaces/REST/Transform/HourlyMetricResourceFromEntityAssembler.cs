using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;
using Flowqueue_Backend.Analytics.Interfaces.REST.Resources;

namespace Flowqueue_Backend.Analytics.Interfaces.REST.Transform;

public static class HourlyMetricResourceFromEntityAssembler
{
    public static HourlyMetricResource ToResourceFromEntity(HourlyMetric entity) =>
        new(
            entity.PeriodStart,
            entity.BranchOfficeId,
            entity.ServiceId,
            entity.TotalTurns,
            entity.CompletedTurns,
            entity.CancelledTurns,
            entity.AverageWaitingMinutes);
}
