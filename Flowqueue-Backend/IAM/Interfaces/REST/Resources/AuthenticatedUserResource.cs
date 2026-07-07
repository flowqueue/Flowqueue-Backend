namespace Flowqueue_Backend.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(
    UserResource User,
    string Token);
