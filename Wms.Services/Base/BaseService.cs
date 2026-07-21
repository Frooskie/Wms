using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;

namespace Wms.Services.Base;

public abstract class BaseService<T>(IRepository<T> repository) : IBaseService<T>
    where T : class
{
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await repository.GetAllAsync(cancellationToken);

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken);

    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        repository.Update(entity);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            repository.Delete(entity);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}