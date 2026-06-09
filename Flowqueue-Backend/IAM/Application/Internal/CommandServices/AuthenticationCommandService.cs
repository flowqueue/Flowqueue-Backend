using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.IAM.Application.Internal.CommandServices;

public class AuthenticationCommandService(
    IUserRepository userRepository,
    ILogger<AuthenticationCommandService> logger) : IAuthenticationCommandService
{
    public async Task<Result<User, SignInUserError>> Handle(
        SignInUserCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = User.NormalizeEmail(command.Email);
            var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
            if (user is null)
                return new Result<User, SignInUserError>.Failure(SignInUserError.InvalidCredentials);

            var passwordHash = PasswordHashingService.Hash(command.Password);
            if (!user.HasPasswordHash(passwordHash))
                return new Result<User, SignInUserError>.Failure(SignInUserError.InvalidCredentials);

            return new Result<User, SignInUserError>.Success(user);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid sign in request");
            return new Result<User, SignInUserError>.Failure(SignInUserError.InvalidCredentials);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error signing in user");
            return new Result<User, SignInUserError>.Failure(SignInUserError.UnexpectedError);
        }
    }
}
