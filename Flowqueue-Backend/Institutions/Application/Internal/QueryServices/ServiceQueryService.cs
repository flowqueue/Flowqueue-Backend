using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;
using Flowqueue_Backend.Institutions.Domain.Repositories;

namespace Flowqueue_Backend.Institutions.Application.Internal.QueryServices;

public class ServiceQueryService(IServiceRepository serviceRepository) : IServiceQueryService
{
    public async Task<IEnumerable<Service>> Handle(
        GetAllServicesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.BranchOfficeId is null)
            return await serviceRepository.ListAsync(cancellationToken);

        return await serviceRepository.FindByBranchOfficeIdAsync(query.BranchOfficeId.Value, cancellationToken);
    }

    public async Task<Service?> Handle(
        GetServiceByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await serviceRepository.FindByIdAsync(query.Id, cancellationToken);
}
