using Flowqueue_Backend.Notifications.Application.Errors;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.Notifications.Application.Services;

public interface INotificationCommandService
{
    Task<Result<Notification, CreateNotificationError>> Handle(
        CreateNotificationCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Notification, UpdateNotificationError>> Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Notification, UpdateNotificationError>> Handle(
        ArchiveNotificationCommand command,
        CancellationToken cancellationToken = default);
}
