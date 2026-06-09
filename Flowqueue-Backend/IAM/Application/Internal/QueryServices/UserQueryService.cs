using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Queries;
using Flowqueue_Backend.IAM.Domain.Repositories;

namespace Flowqueue_Backend.IAM.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<IEnumerable<User>> Handle(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default) =>
        await userRepository.FindByFiltersAsync(query.Role, cancellationToken);

    public async Task<User?> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await userRepository.FindByIdAsync(query.UserId, cancellationToken);
}
