using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Application.Internal.CommandServices;

public class ServiceCommandService(
    IServiceRepository serviceRepository,
    IBranchOfficeRepository branchOfficeRepository,
    IUnitOfWork unitOfWork,
    ILogger<ServiceCommandService> logger) : IServiceCommandService
{
    public async Task<Result<Service, CreateServiceError>> Handle(
        CreateServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var branchOffice = await branchOfficeRepository.FindByIdAsync(command.BranchOfficeId, cancellationToken);
        if (branchOffice is null)
            return new Result<Service, CreateServiceError>.Failure(CreateServiceError.BranchOfficeNotFound);

        var existing = await serviceRepository.FindByBranchOfficeIdAndNameAsync(
            command.BranchOfficeId,
            command.Name,
            cancellationToken);

        if (existing is not null)
        {
            logger.LogWarning("Duplicate service {Name} for branch office {BranchOfficeId}", command.Name, command.BranchOfficeId);
            return new Result<Service, CreateServiceError>.Failure(CreateServiceError.DuplicateService);
        }

        try
        {
            var service = new Service(command);
            await serviceRepository.AddAsync(service, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Service, CreateServiceError>.Success(service);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create service {Name}", command.Name);
            return new Result<Service, CreateServiceError>.Failure(CreateServiceError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating service");
            return new Result<Service, CreateServiceError>.Failure(CreateServiceError.UnexpectedError);
        }
    }
}
