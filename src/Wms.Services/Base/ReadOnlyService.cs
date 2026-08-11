using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Services.Base;

public abstract class ReadOnlyService<T>(IRepository<T> repository) : IReadOnlyService<T>
    where T : class
{
    protected readonly IRepository<T> _repository = repository;

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }
}