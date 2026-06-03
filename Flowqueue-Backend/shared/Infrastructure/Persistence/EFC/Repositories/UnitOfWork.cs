using Flowqueue_Backend.shared.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
namespace Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
 public async Task CompleteAsync(CancellationToken cancellationToken = default)
  => await context.SaveChangesAsync(cancellationToken);

}