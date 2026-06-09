using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;

namespace Flowqueue_Backend.Institutions.Application.Services;

public interface IInstitutionQueryService
{
    Task<IEnumerable<Institution>> Handle(
        GetAllInstitutionsQuery query,
        CancellationToken cancellationToken = default);

    Task<Institution?> Handle(
        GetInstitutionByIdQuery query,
        CancellationToken cancellationToken = default);
}
