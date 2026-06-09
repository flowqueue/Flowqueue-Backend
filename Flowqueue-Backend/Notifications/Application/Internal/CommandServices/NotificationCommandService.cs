using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.Notifications.Application.Errors;
using Flowqueue_Backend.Notifications.Application.Services;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.Notifications.Domain.Repositories;
using Flowqueue_Backend.Queueing.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Notifications.Application.Internal.CommandServices;

public class NotificationCommandService(
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    ITurnRepository turnRepository,
    IUnitOfWork unitOfWork,
    ILogger<NotificationCommandService> logger) : INotificationCommandService
{
    public async Task<Result<Notification, CreateNotificationError>> Handle(
        CreateNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return new Result<Notification, CreateNotificationError>.Failure(CreateNotificationError.UserNotFound);

        if (command.TurnId.HasValue)
        {
            var turn = await turnRepository.FindByIdAsync(command.TurnId.Value, cancellationToken);
            if (turn is null)
                return new Result<Notification, CreateNotificationError>.Failure(CreateNotificationError.TurnNotFound);
        }

        try
        {
            var notification = new Notification(command);
            await notificationRepository.AddAsync(notification, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Notification, CreateNotificationError>.Success(notification);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid notification request");
            return new Result<Notification, CreateNotificationError>.Failure(CreateNotificationError.InvalidNotificationData);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create notification for user {UserId}", command.UserId);
            return new Result<Notification, CreateNotificationError>.Failure(CreateNotificationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating notification");
            return new Result<Notification, CreateNotificationError>.Failure(CreateNotificationError.UnexpectedError);
        }
    }

    public async Task<Result<Notification, UpdateNotificationError>> Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateNotification(command.NotificationId, notification => notification.MarkAsRead(), "mark as read", cancellationToken);

    public async Task<Result<Notification, UpdateNotificationError>> Handle(
        ArchiveNotificationCommand command,
        CancellationToken cancellationToken = default) =>
        await UpdateNotification(command.NotificationId, notification => notification.Archive(), "archive", cancellationToken);

    private async Task<Result<Notification, UpdateNotificationError>> UpdateNotification(
        int notificationId,
        Action<Notification> updateAction,
        string operationName,
        CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.FindByIdAsync(notificationId, cancellationToken);
        if (notification is null)
            return new Result<Notification, UpdateNotificationError>.Failure(UpdateNotificationError.NotificationNotFound);

        try
        {
            updateAction(notification);
            notificationRepository.Update(notification);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Notification, UpdateNotificationError>.Success(notification);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid notification status change while trying to {OperationName} notification {NotificationId}", operationName, notificationId);
            return new Result<Notification, UpdateNotificationError>.Failure(UpdateNotificationError.InvalidStatusChange);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not {OperationName} notification {NotificationId}", operationName, notificationId);
            return new Result<Notification, UpdateNotificationError>.Failure(UpdateNotificationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while trying to {OperationName} notification {NotificationId}", operationName, notificationId);
            return new Result<Notification, UpdateNotificationError>.Failure(UpdateNotificationError.UnexpectedError);
        }
    }
}
