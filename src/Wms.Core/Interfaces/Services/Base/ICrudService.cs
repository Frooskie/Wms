namespace Wms.Core.Interfaces.Services.Base;

public interface ICrudService<TEntity> : IReadOnlyService<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}