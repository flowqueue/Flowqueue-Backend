using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Application.Internal.CommandServices;

public class BranchOfficeCommandService(
    IBranchOfficeRepository branchOfficeRepository,
    IInstitutionRepository institutionRepository,
    IUnitOfWork unitOfWork,
    ILogger<BranchOfficeCommandService> logger) : IBranchOfficeCommandService
{
    public async Task<Result<BranchOffice, CreateBranchOfficeError>> Handle(
        CreateBranchOfficeCommand command,
        CancellationToken cancellationToken = default)
    {
        var institution = await institutionRepository.FindByIdAsync(command.InstitutionId, cancellationToken);
        if (institution is null)
            return new Result<BranchOffice, CreateBranchOfficeError>.Failure(CreateBranchOfficeError.InstitutionNotFound);

        var existing = await branchOfficeRepository.FindByInstitutionIdAndNameAsync(
            command.InstitutionId,
            command.Name,
            cancellationToken);

        if (existing is not null)
        {
            logger.LogWarning("Duplicate branch office {Name} for institution {InstitutionId}", command.Name, command.InstitutionId);
            return new Result<BranchOffice, CreateBranchOfficeError>.Failure(CreateBranchOfficeError.DuplicateBranchOffice);
        }

        try
        {
            var branchOffice = new BranchOffice(command);
            await branchOfficeRepository.AddAsync(branchOffice, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<BranchOffice, CreateBranchOfficeError>.Success(branchOffice);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create branch office {Name}", command.Name);
            return new Result<BranchOffice, CreateBranchOfficeError>.Failure(CreateBranchOfficeError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating branch office");
            return new Result<BranchOffice, CreateBranchOfficeError>.Failure(CreateBranchOfficeError.UnexpectedError);
        }
    }
}
