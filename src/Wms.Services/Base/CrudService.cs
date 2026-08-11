using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Services.Base;

public abstract class CrudService<T>(IRepository<T> repository) : ReadOnlyService<T>(repository), ICrudService<T>
    where T : class
{
    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}