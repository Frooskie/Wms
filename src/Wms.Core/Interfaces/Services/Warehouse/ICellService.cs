using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Core.Interfaces.Services.Warehouse;

public interface ICellService : ICrudService<Cell>
{
    Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default);
}