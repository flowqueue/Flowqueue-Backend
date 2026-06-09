using System.Net.Mime;
using Flowqueue_Backend.Notifications.Application.Services;
using Flowqueue_Backend.Notifications.Domain.Model.Commands;
using Flowqueue_Backend.Notifications.Domain.Model.Queries;
using Flowqueue_Backend.Notifications.Interfaces.REST.Resources;
using Flowqueue_Backend.Notifications.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowqueue_Backend.Notifications.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Notifications")]
public class NotificationsController(
    INotificationCommandService notificationCommandService,
    INotificationQueryService notificationQueryService,
    ILogger<NotificationsController> logger) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a notification", OperationId = "CreateNotification")]
    [SwaggerResponse(201, "Created", typeof(NotificationResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<NotificationResource>> CreateNotification(
        [FromBody] CreateNotificationResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateNotificationCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await notificationCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateNotificationResultAssembler
                .ToActionResultFromCreateNotificationResult(result, this, nameof(GetNotificationById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid notification request");
            return BadRequest("Invalid notification request.");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets notifications", OperationId = "GetNotifications")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<NotificationResource>))]
    public async Task<ActionResult<IEnumerable<NotificationResource>>> GetNotifications(
        [FromQuery] int? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllNotificationsQuery(userId, status, type);
        var result = await notificationQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a notification by id", OperationId = "GetNotificationById")]
    [SwaggerResponse(200, "OK", typeof(NotificationResource))]
    [SwaggerResponse(404, "Not Found")]
    public async Task<ActionResult<NotificationResource>> GetNotificationById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationByIdQuery(id);
        var result = await notificationQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpGet("/api/v1/users/{userId:int}/notifications")]
    [SwaggerOperation(Summary = "Gets notifications by user id", OperationId = "GetNotificationsByUserId")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<NotificationResource>))]
    public async Task<ActionResult<IEnumerable<NotificationResource>>> GetNotificationsByUserId(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationsByUserIdQuery(userId);
        var result = await notificationQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPatch("{id}/mark-as-read")]
    [SwaggerOperation(Summary = "Marks a notification as read", OperationId = "MarkNotificationAsRead")]
    [SwaggerResponse(200, "OK", typeof(NotificationResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<NotificationResource>> MarkNotificationAsRead(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationCommandService.Handle(new MarkNotificationAsReadCommand(id), cancellationToken);
        return ActionResultFromUpdateNotificationResultAssembler.ToActionResultFromUpdateNotificationResult(result, this);
    }

    [HttpPatch("{id}/archive")]
    [SwaggerOperation(Summary = "Archives a notification", OperationId = "ArchiveNotification")]
    [SwaggerResponse(200, "OK", typeof(NotificationResource))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    public async Task<ActionResult<NotificationResource>> ArchiveNotification(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationCommandService.Handle(new ArchiveNotificationCommand(id), cancellationToken);
        return ActionResultFromUpdateNotificationResultAssembler.ToActionResultFromUpdateNotificationResult(result, this);
    }
}
