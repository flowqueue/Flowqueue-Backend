using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.IAM.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> FindByFiltersAsync(string? role, CancellationToken cancellationToken = default);
}
