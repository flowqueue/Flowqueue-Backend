using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Infrastructure.Persistence.EFC.Repositories;

public class BranchOfficeRepository(AppDbContext context) : BaseRepository<BranchOffice>(context), IBranchOfficeRepository
{
    public async Task AddAsync(BranchOffice branchOffice, CancellationToken cancellationToken = default) =>
        await Context.Set<BranchOffice>().AddAsync(branchOffice, cancellationToken);

    public async Task<IEnumerable<BranchOffice>> FindByInstitutionIdAsync(
        int institutionId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<BranchOffice>()
            .Where(branchOffice => branchOffice.InstitutionId == institutionId)
            .ToListAsync(cancellationToken);

    public async Task<BranchOffice?> FindByInstitutionIdAndNameAsync(
        int institutionId,
        string name,
        CancellationToken cancellationToken = default) =>
        await Context.Set<BranchOffice>()
            .FirstOrDefaultAsync(
                branchOffice => branchOffice.InstitutionId == institutionId && branchOffice.Name == name.Trim(),
                cancellationToken);
}
