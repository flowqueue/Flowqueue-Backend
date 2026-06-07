using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Infrastructure.Persistence.EFC.Repositories;

public class ServiceRepository(AppDbContext context) : BaseRepository<Service>(context), IServiceRepository
{
    public async Task AddAsync(Service service, CancellationToken cancellationToken = default) =>
        await Context.Set<Service>().AddAsync(service, cancellationToken);

    public async Task<IEnumerable<Service>> FindByBranchOfficeIdAsync(
        int branchOfficeId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Service>()
            .Where(service => service.BranchOfficeId == branchOfficeId)
            .ToListAsync(cancellationToken);

    public async Task<Service?> FindByBranchOfficeIdAndNameAsync(
        int branchOfficeId,
        string name,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Service>()
            .FirstOrDefaultAsync(
                service => service.BranchOfficeId == branchOfficeId && service.Name == name.Trim(),
                cancellationToken);
}
