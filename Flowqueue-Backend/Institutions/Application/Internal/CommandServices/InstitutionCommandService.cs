using Flowqueue_Backend.Institutions.Application.Errors;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.Commands;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.shared.Application.Patterns;
using Flowqueue_Backend.shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.Institutions.Application.Internal.CommandServices;

public class InstitutionCommandService(
    IInstitutionRepository institutionRepository,
    IUnitOfWork unitOfWork,
    ILogger<InstitutionCommandService> logger) : IInstitutionCommandService
{
    public async Task<Result<Institution, CreateInstitutionError>> Handle(
        CreateInstitutionCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await institutionRepository.FindByNameAsync(command.Name, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Duplicate institution {Name}", command.Name);
            return new Result<Institution, CreateInstitutionError>.Failure(CreateInstitutionError.DuplicateInstitution);
        }

        try
        {
            var institution = new Institution(command);
            await institutionRepository.AddAsync(institution, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Institution, CreateInstitutionError>.Success(institution);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not create institution {Name}", command.Name);
            return new Result<Institution, CreateInstitutionError>.Failure(CreateInstitutionError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating institution");
            return new Result<Institution, CreateInstitutionError>.Failure(CreateInstitutionError.UnexpectedError);
        }
    }
}
