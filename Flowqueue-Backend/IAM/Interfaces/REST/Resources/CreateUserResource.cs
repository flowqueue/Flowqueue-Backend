namespace Flowqueue_Backend.IAM.Interfaces.REST.Resources;

public record CreateUserResource(
    string FullName,
    string Email,
    string Password,
    string Role,
    string? DocumentNumber);
