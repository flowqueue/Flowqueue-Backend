using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;
using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.IAM.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public new async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await Context.Set<User>().AddAsync(user, cancellationToken);
    public async Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = User.NormalizeEmail(email);
        return await Context.Set<User>()
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> FindByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedDocument = documentNumber.Trim();
        return await Context.Set<User>()
            .FirstOrDefaultAsync(user => user.DocumentNumber == normalizedDocument, cancellationToken);
    }

    public async Task<IEnumerable<User>> FindByFiltersAsync(
        string? role,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<User>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
        {
            UserRole normalizedRole;
            try
            {
                normalizedRole = new UserRole(role);
            }
            catch (ArgumentException)
            {
                return [];
            }

            query = query.Where(user => user.Role == normalizedRole);
        }

        return await query
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);
    }
}
