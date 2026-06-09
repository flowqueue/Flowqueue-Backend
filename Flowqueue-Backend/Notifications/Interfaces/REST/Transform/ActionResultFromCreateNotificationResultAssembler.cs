using Flowqueue_Backend.Notifications.Application.Errors;
using Flowqueue_Backend.Notifications.Domain.Model.Aggregates;
using Flowqueue_Backend.Notifications.Interfaces.REST.Resources;
using Flowqueue_Backend.shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace Flowqueue_Backend.Notifications.Interfaces.REST.Transform;

public static class ActionResultFromCreateNotificationResultAssembler
{
    public static ActionResult<NotificationResource> ToActionResultFromCreateNotificationResult(
        Result<Notification, CreateNotificationError> result,
        ControllerBase controller,
        string actionName) =>
        result switch
        {
            Result<Notification, CreateNotificationError>.Success success => controller.CreatedAtAction(
                actionName,
                new { id = success.Value.Id },
                NotificationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Notification, CreateNotificationError>.Failure failure => failure.Error switch
            {
                CreateNotificationError.UserNotFound => controller.NotFound("User was not found."),
                CreateNotificationError.TurnNotFound => controller.NotFound("Turn was not found."),
                CreateNotificationError.InvalidNotificationData => controller.BadRequest("Invalid notification request."),
                _ => controller.Problem(statusCode: 500, detail: "Notification could not be created.")
            },
            _ => controller.Problem(statusCode: 500, detail: "Notification could not be created.")
        };
}
