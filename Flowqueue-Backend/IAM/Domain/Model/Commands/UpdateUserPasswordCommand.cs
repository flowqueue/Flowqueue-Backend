namespace Flowqueue_Backend.IAM.Domain.Model.Commands;

public record UpdateUserPasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword);
