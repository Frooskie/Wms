using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class CellRepository(ApplicationDbContext context) : Repository<Cell>(context), ICellRepository
{
    public async Task<Cell?> GetCellWithShelfAsync(int cellId, CancellationToken cancellationToken = default)
    {
        return await Context.Cells
            .Include(c => c.Shelf)
            .FirstOrDefaultAsync(c => c.Id == cellId, cancellationToken);
    }

    public async Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default)
    {
        var cell = await Context.Cells.FindAsync([cellId], cancellationToken);
        return cell?.IsOccupied ?? false;
    }
}