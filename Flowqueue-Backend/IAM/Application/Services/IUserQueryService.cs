using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Queries;

namespace Flowqueue_Backend.IAM.Application.Services;

public interface IUserQueryService
{
    Task<IEnumerable<User>> Handle(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default);

    Task<User?> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default);
}
