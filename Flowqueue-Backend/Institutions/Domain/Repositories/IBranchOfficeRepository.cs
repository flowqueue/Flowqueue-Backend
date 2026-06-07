using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Domain.Repositories;

public interface IBranchOfficeRepository : IBaseRepository<BranchOffice>
{
    Task AddAsync(BranchOffice branchOffice, CancellationToken cancellationToken = default);
    Task<IEnumerable<BranchOffice>> FindByInstitutionIdAsync(int institutionId, CancellationToken cancellationToken = default);
    Task<BranchOffice?> FindByInstitutionIdAndNameAsync(int institutionId, string name, CancellationToken cancellationToken = default);
}
