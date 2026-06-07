using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.Institutions.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Application.Internal.QueryServices;

public class InstitutionQueryService(IInstitutionRepository institutionRepository) : IInstitutionQueryService
{
    public async Task<IEnumerable<Institution>> Handle(
        GetAllInstitutionsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Type))
            return await institutionRepository.ListAsync(cancellationToken);

        return await institutionRepository.FindByTypeAsync(new InstitutionType(query.Type), cancellationToken);
    }

    public async Task<Institution?> Handle(
        GetInstitutionByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await institutionRepository.FindByIdAsync(query.Id, cancellationToken);
}
