using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;

namespace Flowqueue_Backend.Institutions.Application.Services;

public interface IServiceQueryService
{
    Task<IEnumerable<Service>> Handle(
        GetAllServicesQuery query,
        CancellationToken cancellationToken = default);

    Task<Service?> Handle(
        GetServiceByIdQuery query,
        CancellationToken cancellationToken = default);
}
