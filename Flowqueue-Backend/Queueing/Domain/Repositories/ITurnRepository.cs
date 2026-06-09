using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.Queueing.Domain.Repositories;

public interface ITurnRepository : IBaseRepository<Turn>
{
    Task AddAsync(Turn turn, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turn>> FindByFiltersAsync(
        int? branchOfficeId,
        int? serviceId,
        string? status,
        CancellationToken cancellationToken = default);
    Task<int> GetLastTurnNumberByServiceIdAsync(int serviceId, CancellationToken cancellationToken = default);
}
