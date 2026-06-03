namespace Flowqueue_Backend.shared.Domain.Repositories;

public interface IBaseRepository<TEntity>
{
    Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default);
}