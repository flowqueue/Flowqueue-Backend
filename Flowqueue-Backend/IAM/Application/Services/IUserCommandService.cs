using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.IAM.Application.Services;

public interface IUserCommandService
{
    Task<Result<User, CreateUserError>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<User, UpdateUserError>> Handle(
        UpdateUserPasswordCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<User, DeleteUserError>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default);
}
