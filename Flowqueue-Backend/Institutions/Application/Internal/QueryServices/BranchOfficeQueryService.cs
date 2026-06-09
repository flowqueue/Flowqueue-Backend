using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;
using Flowqueue_Backend.Institutions.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Application.Internal.QueryServices;

public class BranchOfficeQueryService(IBranchOfficeRepository branchOfficeRepository) : IBranchOfficeQueryService
{
    public async Task<IEnumerable<BranchOffice>> Handle(
        GetAllBranchOfficesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.InstitutionId is null)
            return await branchOfficeRepository.ListAsync(cancellationToken);

        return await branchOfficeRepository.FindByInstitutionIdAsync(query.InstitutionId.Value, cancellationToken);
    }

    public async Task<BranchOffice?> Handle(
        GetBranchOfficeByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await branchOfficeRepository.FindByIdAsync(query.Id, cancellationToken);
}
