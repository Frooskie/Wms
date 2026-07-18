using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;

namespace Wms.Services.Base;

public abstract class BaseService<T> : IBaseService<T> where T : class
{
    protected readonly IRepository<T> _repository;

    protected BaseService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(id, cancellationToken);

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