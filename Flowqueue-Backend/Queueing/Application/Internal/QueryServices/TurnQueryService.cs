using Flowqueue_Backend.Queueing.Application.Services;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.Queries;
using Flowqueue_Backend.Queueing.Domain.Repositories;

namespace Flowqueue_Backend.Queueing.Application.Internal.QueryServices;

public class TurnQueryService(ITurnRepository turnRepository) : ITurnQueryService
{
    public async Task<IEnumerable<Turn>> Handle(
        GetAllTurnsQuery query,
        CancellationToken cancellationToken = default) =>
        await turnRepository.FindByFiltersAsync(
            query.BranchOfficeId,
            query.ServiceId,
            query.Status,
            cancellationToken);

    public async Task<Turn?> Handle(
        GetTurnByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await turnRepository.FindByIdAsync(query.Id, cancellationToken);
}
