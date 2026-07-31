using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface ICellService : ICrudService<Cell>
{
    Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default);
}