using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Infrastructure.Persistence.EFC.Repositories;

public class InstitutionRepository(AppDbContext context) : BaseRepository<Institution>(context), IInstitutionRepository
{
    public async Task AddAsync(Institution institution, CancellationToken cancellationToken = default) =>
        await Context.Set<Institution>().AddAsync(institution, cancellationToken);

    public async Task<Institution?> FindByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await Context.Set<Institution>()
            .FirstOrDefaultAsync(institution => institution.Name == name.Trim(), cancellationToken);

    public async Task<IEnumerable<Institution>> FindByTypeAsync(
        InstitutionType type,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Institution>()
            .Where(institution => institution.Type == type)
            .ToListAsync(cancellationToken);
}
