namespace Flowqueue_Backend.IAM.Interfaces.REST.Resources;

public record UpdateUserPasswordResource(
    string CurrentPassword,
    string NewPassword);
