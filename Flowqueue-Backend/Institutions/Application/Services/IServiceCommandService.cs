using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.shared.Application.Patterns;

namespace Flowqueue_Backend.Institutions.Application.Services;

public interface IServiceCommandService
{
    Task<Result<Service, CreateServiceError>> Handle(
        CreateServiceCommand command,
        CancellationToken cancellationToken = default);
}
