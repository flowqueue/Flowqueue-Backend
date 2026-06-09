using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Domain.Repositories;

public interface IServiceRepository : IBaseRepository<Service>
{
    Task AddAsync(Service service, CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> FindByBranchOfficeIdAsync(int branchOfficeId, CancellationToken cancellationToken = default);
    Task<Service?> FindByBranchOfficeIdAndNameAsync(int branchOfficeId, string name, CancellationToken cancellationToken = default);
}
