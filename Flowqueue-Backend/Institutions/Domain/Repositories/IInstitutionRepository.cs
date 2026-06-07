using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Domain.Repositories;

public interface IInstitutionRepository : IBaseRepository<Institution>
{
    Task AddAsync(Institution institution, CancellationToken cancellationToken = default);
    Task<Institution?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Institution>> FindByTypeAsync(InstitutionType type, CancellationToken cancellationToken = default);
}
