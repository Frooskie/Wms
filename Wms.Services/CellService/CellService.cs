using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.CellService;

public class CellService(IRepository<Cell> repository, ICellRepository cellRepository)
    : BaseService<Cell>(repository), ICellService
{
    public async Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default)
        => await cellRepository.IsCellOccupiedAsync(cellId, cancellationToken);
}