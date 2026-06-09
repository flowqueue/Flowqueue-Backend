using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Queries;

namespace Flowqueue_Backend.Institutions.Application.Services;

public interface IBranchOfficeQueryService
{
    Task<IEnumerable<BranchOffice>> Handle(
        GetAllBranchOfficesQuery query,
        CancellationToken cancellationToken = default);

    Task<BranchOffice?> Handle(
        GetBranchOfficeByIdQuery query,
        CancellationToken cancellationToken = default);
}
