using Flowqueue_Backend.Analytics.Domain.Model.Aggregates;
using Flowqueue_Backend.Analytics.Domain.Repositories;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Analytics.Infrastructure.Persistence.EFC.Repositories;

public class AnalyticsRepository(AppDbContext context) : IAnalyticsRepository
{
    public async Task<AnalyticsSummary> GetSummaryAsync(
        int? branchOfficeId = null,
        int? serviceId = null,
        CancellationToken cancellationToken = default)
    {
        var turns = await GetFilteredTurns(branchOfficeId, serviceId, null, null, cancellationToken);
        return BuildSummary(turns);
    }

    public async Task<IEnumerable<HourlyMetric>> GetHourlyMetricsAsync(
        int? branchOfficeId = null,
        int? serviceId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        var turns = await GetFilteredTurns(branchOfficeId, serviceId, from, to, cancellationToken);

        return turns
            .GroupBy(turn => new
            {
                PeriodStart = new DateTimeOffset(
                    turn.RegisteredAt.Year,
                    turn.RegisteredAt.Month,
                    turn.RegisteredAt.Day,
                    turn.RegisteredAt.Hour,
                    0,
                    0,
                    turn.RegisteredAt.Offset),
                turn.BranchOfficeId,
                turn.ServiceId
            })
            .OrderBy(group => group.Key.PeriodStart)
            .ThenBy(group => group.Key.BranchOfficeId)
            .ThenBy(group => group.Key.ServiceId)
            .Select(group => new HourlyMetric(
                group.Key.PeriodStart,
                group.Key.BranchOfficeId,
                group.Key.ServiceId,
                group.Count(),
                group.Count(turn => turn.Status.Value == TurnStatus.Completed),
                group.Count(turn => turn.Status.Value == TurnStatus.Cancelled),
                CalculateAverageWaitingMinutes(group)))
            .ToList();
    }

    private async Task<List<Turn>> GetFilteredTurns(
        int? branchOfficeId,
        int? serviceId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Turn>().AsNoTracking().AsQueryable();

        if (branchOfficeId is not null)
            query = query.Where(turn => turn.BranchOfficeId == branchOfficeId.Value);

        if (serviceId is not null)
            query = query.Where(turn => turn.ServiceId == serviceId.Value);

        if (from is not null)
            query = query.Where(turn => turn.RegisteredAt >= from.Value);

        if (to is not null)
            query = query.Where(turn => turn.RegisteredAt <= to.Value);

        return await query.ToListAsync(cancellationToken);
    }

    private static AnalyticsSummary BuildSummary(IReadOnlyCollection<Turn> turns) =>
        new(
            turns.Count,
            turns.Count(turn => turn.Status.Value == TurnStatus.Waiting),
            turns.Count(turn => turn.Status.Value == TurnStatus.Called),
            turns.Count(turn => turn.Status.Value == TurnStatus.Completed),
            turns.Count(turn => turn.Status.Value == TurnStatus.Cancelled),
            turns.Count(turn => turn.Status.Value == TurnStatus.Absent),
            CalculateAverageWaitingMinutes(turns),
            CalculateAverageServiceMinutes(turns));

    private static decimal CalculateAverageWaitingMinutes(IEnumerable<Turn> turns)
    {
        var values = turns
            .Where(turn => turn.CalledAt is not null)
            .Select(turn => (turn.CalledAt!.Value - turn.RegisteredAt).TotalMinutes)
            .Where(value => value >= 0)
            .ToList();

        return RoundAverage(values);
    }

    private static decimal CalculateAverageServiceMinutes(IEnumerable<Turn> turns)
    {
        var values = turns
            .Where(turn => turn.CalledAt is not null && turn.CompletedAt is not null)
            .Select(turn => (turn.CompletedAt!.Value - turn.CalledAt!.Value).TotalMinutes)
            .Where(value => value >= 0)
            .ToList();

        return RoundAverage(values);
    }

    private static decimal RoundAverage(IReadOnlyCollection<double> values)
    {
        if (values.Count == 0) return 0;
        return Math.Round((decimal)values.Average(), 2);
    }
}
