using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.Queries;

namespace Flowqueue_Backend.Queueing.Application.Services;

public interface ITurnQueryService
{
    Task<IEnumerable<Turn>> Handle(GetAllTurnsQuery query, CancellationToken cancellationToken = default);
    Task<Turn?> Handle(GetTurnByIdQuery query, CancellationToken cancellationToken = default);
}
