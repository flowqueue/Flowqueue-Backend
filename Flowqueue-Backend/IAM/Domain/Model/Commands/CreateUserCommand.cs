using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;

namespace Flowqueue_Backend.IAM.Domain.Model.Commands;

public record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    string? DocumentNumber);
