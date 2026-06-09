using Flowqueue_Backend.Queueing.Application.Errors;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.Commands;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.Queueing.Application.Services;

public interface ITurnCommandService
{
    Task<Result<Turn, CreateTurnError>> Handle(CreateTurnCommand command, CancellationToken cancellationToken = default);
    Task<Result<Turn, UpdateTurnError>> Handle(CallTurnCommand command, CancellationToken cancellationToken = default);
    Task<Result<Turn, UpdateTurnError>> Handle(CompleteTurnCommand command, CancellationToken cancellationToken = default);
    Task<Result<Turn, UpdateTurnError>> Handle(CancelTurnCommand command, CancellationToken cancellationToken = default);
    Task<Result<Turn, UpdateTurnError>> Handle(MarkTurnAsAbsentCommand command, CancellationToken cancellationToken = default);
}
