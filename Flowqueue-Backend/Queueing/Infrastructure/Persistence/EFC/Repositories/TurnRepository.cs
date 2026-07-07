using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;
using Flowqueue_Backend.Queueing.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Queueing.Infrastructure.Persistence.EFC.Repositories;

public class TurnRepository(AppDbContext context) : BaseRepository<Turn>(context), ITurnRepository
{
    public async Task<IEnumerable<Turn>> FindByFiltersAsync(
        int? branchOfficeId,
        int? serviceId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Turn>().AsQueryable();

        if (branchOfficeId is not null)
            query = query.Where(turn => turn.BranchOfficeId == branchOfficeId.Value);

        if (serviceId is not null)
            query = query.Where(turn => turn.ServiceId == serviceId.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            TurnStatus normalizedStatus;
            try
            {
                normalizedStatus = new TurnStatus(status);
            }
            catch (ArgumentException)
            {
                return [];
            }

            query = query.Where(turn => turn.Status == normalizedStatus);
        }

        return await query
            .OrderByDescending(turn => turn.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetLastTurnNumberByServiceIdAsync(
        int serviceId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Turn>()
            .Where(turn => turn.ServiceId == serviceId)
            .Select(turn => (int?)turn.TurnNumber)
            .MaxAsync(cancellationToken) ?? 0;
}
