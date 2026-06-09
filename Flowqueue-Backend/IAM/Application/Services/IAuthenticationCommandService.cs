using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.IAM.Application.Services;

public interface IAuthenticationCommandService
{
    Task<Result<User, SignInUserError>> Handle(
        SignInUserCommand command,
        CancellationToken cancellationToken = default);
}
