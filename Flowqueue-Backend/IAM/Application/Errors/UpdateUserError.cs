namespace Flowqueue_Backend.IAM.Application.Errors;

public enum UpdateUserError
{
    UserNotFound,
    InvalidCurrentPassword,
    InvalidUserData,
    UnexpectedError
}
