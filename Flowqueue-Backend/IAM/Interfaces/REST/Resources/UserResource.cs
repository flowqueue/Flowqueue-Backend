namespace Flowqueue_Backend.IAM.Interfaces.REST.Resources;

public record UserResource(
    int Id,
    string FullName,
    string Email,
    string Role,
    string? DocumentNumber,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
