using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.Notifications.Domain.Model.ValueObjects;
using Flowqueue_Backend.Notifications.Domain.Repositories;
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
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
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
            var ticketCode = new TicketCode($"{service.Prefix.Value}{command.ServiceId:00}-{nextTurnNumber:000}");
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
        await UpdateTurnStatus(
            command.TurnId,
            turn => turn.Call(),
            "call",
            turn => new CreateNotificationCommand(
                0,
                turn.Id,
                "¡Es tu turno!",
                $"Tu turno {turn.TicketCode.Value} está siendo llamado. Acércate a la ventanilla.",
                NotificationType.NewTurnCalled()),
            cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        CompleteTurnCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(
            command.TurnId,
            turn => turn.Complete(),
            "complete",
            turn => new CreateNotificationCommand(
                0,
                turn.Id,
                "Atención completada",
                $"Tu turno {turn.TicketCode.Value} fue atendido correctamente. ¡Gracias por tu visita!",
                NotificationType.NewTurnCompleted()),
            cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        CancelTurnCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(
            command.TurnId,
            turn => turn.Cancel(),
            "cancel",
            turn => new CreateNotificationCommand(
                0,
                turn.Id,
                "Turno cancelado",
                $"Tu turno {turn.TicketCode.Value} fue cancelado.",
                NotificationType.NewTurnCancelled()),
            cancellationToken);

    public async Task<Result<Turn, UpdateTurnError>> Handle(
        MarkTurnAsAbsentCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateTurnStatus(
            command.TurnId,
            turn => turn.MarkAsAbsent(),
            "mark as absent",
            turn => new CreateNotificationCommand(
                0,
                turn.Id,
                "Turno marcado como ausente",
                $"No respondiste al llamado del turno {turn.TicketCode.Value}. Genera un nuevo ticket si aún necesitas atención.",
                NotificationType.NewGeneral()),
            cancellationToken);

    private async Task<Result<Turn, UpdateTurnError>> UpdateTurnStatus(
        int turnId,
        Action<Turn> updateAction,
        string operationName,
        Func<Turn, CreateNotificationCommand> notificationFactory,
        CancellationToken cancellationToken)
    {
        var turn = await turnRepository.FindByIdAsync(turnId, cancellationToken);
        if (turn is null)
            return new Result<Turn, UpdateTurnError>.Failure(UpdateTurnError.TurnNotFound);

        try
        {
            updateAction(turn);
            turnRepository.Update(turn);
            await AddCitizenNotificationAsync(turn, notificationFactory, cancellationToken);
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

    private async Task AddCitizenNotificationAsync(
        Turn turn,
        Func<Turn, CreateNotificationCommand> notificationFactory,
        CancellationToken cancellationToken)
    {
        try
        {
            var citizen = await userRepository.FindByDocumentNumberAsync(turn.CitizenDocumentNumber, cancellationToken);
            if (citizen is null) return;

            var command = notificationFactory(turn) with { UserId = citizen.Id };
            await notificationRepository.AddAsync(new Notification(command), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not queue notification for turn {TurnId}", turn.Id);
        }
    }
}
