using Flowqueue_Backend.IAM.Application.Errors;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserCommandService> logger) : IUserCommandService
{
    public async Task<Result<User, CreateUserError>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = User.NormalizeEmail(command.Email);
            var existing = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
            if (existing is not null)
            {
                logger.LogWarning("Duplicate user email {Email}", normalizedEmail);
                return new Result<User, CreateUserError>.Failure(CreateUserError.DuplicateEmail);
            }

            var passwordHash = PasswordHashingService.Hash(command.Password);
            var user = new User(command, passwordHash);

            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, CreateUserError>.Success(user);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid user request");
            return new Result<User, CreateUserError>.Failure(CreateUserError.InvalidUserData);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create user {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating user");
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
    }

    public async Task<Result<User, UpdateUserError>> Handle(
        UpdateUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return new Result<User, UpdateUserError>.Failure(UpdateUserError.UserNotFound);

        try
        {
            if (!user.HasPasswordHash(PasswordHashingService.Hash(command.CurrentPassword)))
                return new Result<User, UpdateUserError>.Failure(UpdateUserError.InvalidCurrentPassword);

            user.ChangePasswordHash(PasswordHashingService.Hash(command.NewPassword));
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, UpdateUserError>.Success(user);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid password update request for user {UserId}", command.UserId);
            return new Result<User, UpdateUserError>.Failure(UpdateUserError.InvalidUserData);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not update password for user {UserId}", command.UserId);
            return new Result<User, UpdateUserError>.Failure(UpdateUserError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating password for user {UserId}", command.UserId);
            return new Result<User, UpdateUserError>.Failure(UpdateUserError.UnexpectedError);
        }
    }

    public async Task<Result<User, DeleteUserError>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return new Result<User, DeleteUserError>.Failure(DeleteUserError.UserNotFound);

        try
        {
            userRepository.Remove(user);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, DeleteUserError>.Success(user);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not delete user {UserId}", command.UserId);
            return new Result<User, DeleteUserError>.Failure(DeleteUserError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting user {UserId}", command.UserId);
            return new Result<User, DeleteUserError>.Failure(DeleteUserError.UnexpectedError);
        }
    }
}
