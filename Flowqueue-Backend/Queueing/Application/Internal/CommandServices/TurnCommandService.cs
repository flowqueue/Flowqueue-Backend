using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.Queueing.Application.Errors;
using Flowqueue_Backend.Queueing.Application.Services;
using Flowqueue_Backend.Queueing.Domain.Model.Aggregates;
using Flowqueue_Backend.Queueing.Domain.Model.Commands;
using Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;
using Flowqueue_Backend.Queueing.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Queueing.Application.Internal.CommandServices;

public class TurnCommandService(
    ITurnRepository turnRepository,
    IBranchOfficeRepository branchOfficeRepository,
    IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,
    ILogger<TurnCommandService> logger) : ITurnCommandService
{
    public async Task<Result<Turn, CreateTurnError>> Handle(
        CreateTurnCommand command,
        CancellationToken cancellationToken = default)
    {
        var branchOffice = await branchOfficeRepository.FindByIdAsync(command.BranchOfficeId, cancellationToken);
        if (branchOffice is null)
            return new Result<Turn, CreateTurnError>.Failure(CreateTurnError.BranchOfficeNotFound);

        var service = await serviceRepository.FindByIdAsync(command.ServiceId, cancellationToken);
        if (service is null)
            return new Result<Turn, CreateTurnError>.Failure(CreateTurnError.ServiceNotFound);

        if (service.BranchOfficeId != command.BranchOfficeId)
            return new Result<Turn, CreateTurnError>.Failure(CreateTurnError.ServiceDoesNotBelongToBranchOffice);

        try
        {
            var lastTurnNumber = await turnRepository.GetLastTurnNumberByServiceIdAsync(command.ServiceId, cancellationToken);
            var nextTurnNumber = lastTurnNumber + 1;
            var ticketCode = new TicketCode($"{service.Prefix.Value}-{nextTurnNumber:000}");
            var turn = new Turn(command, nextTurnNumber, ticketCode);

            await turnRepository.AddAsync(turn, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Turn, CreateTurnError>.Success(turn);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create turn for service {ServiceId}", command.ServiceId);
            return new Result<Turn, CreateTurnError>.Failure(CreateTurnError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating turn");
            return new Result<Turn, CreateTurnError>.Failure(CreateTurnError.UnexpectedError);
        }
    }

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        CallTurnCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(command.TurnId, turn => turn.Call(), "call", cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        CompleteTurnCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(command.TurnId, turn => turn.Complete(), "complete", cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        CancelTurnCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(command.TurnId, turn => turn.Cancel(), "cancel", cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        MarkTurnAsAbsentCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(command.TurnId, turn => turn.MarkAsAbsent(), "mark as absent", cancellationToken);

    private async Task<Result<Turn, UpdateTurnError>> UpdateTurnStatus(
        int turnId,
        Action<Turn> updateAction,
        string operationName,
        CancellationToken cancellationToken)
    {
        var turn = await turnRepository.FindByIdAsync(turnId, cancellationToken);
        if (turn is null)
            return new Result<Turn, UpdateTurnError>.Failure(UpdateTurnError.TurnNotFound);

        try
        {
            updateAction(turn);
            turnRepository.Update(turn);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Turn, UpdateTurnError>.Success(turn);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid turn status change while trying to {OperationName} turn {TurnId}", operationName, turnId);
            return new Result<Turn, UpdateTurnError>.Failure(UpdateTurnError.InvalidStatusChange);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not {OperationName} turn {TurnId}", operationName, turnId);
            return new Result<Turn, UpdateTurnError>.Failure(UpdateTurnError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while trying to {OperationName} turn {TurnId}", operationName, turnId);
            return new Result<Turn, UpdateTurnError>.Failure(UpdateTurnError.UnexpectedError);
        }
    }
}
