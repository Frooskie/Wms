using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface ICellRepository : IRepository<Cell>
{
    Task<Cell?> GetCellWithShelfAsync(int cellId, CancellationToken cancellationToken = default);
    Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default);
}