using Flowqueue_Backend.Notifications.Application.Errors;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Notifications.Interfaces.REST.Transform;

public static class ActionResultFromUpdateNotificationResultAssembler
{
    public static ActionResult<NotificationResource> ToActionResultFromUpdateNotificationResult(
        Result<Notification, UpdateNotificationError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<Notification, UpdateNotificationError>.Success success =>
                controller.Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Notification, UpdateNotificationError>.Failure failure => failure.Error switch
            {
                UpdateNotificationError.NotificationNotFound => controller.NotFound("Notification was not found."),
                UpdateNotificationError.InvalidStatusChange => controller.BadRequest("Invalid notification status change."),
                _ => controller.Problem(statusCode: 500, detail: "Notification could not be updated.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Notification could not be updated.")
        };
}
