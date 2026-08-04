using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class ReceiptRepository(ApplicationDbContext context) : Repository<Receipt>(context), IReceiptRepository
{
    public async Task<Receipt?> GetReceiptWithLinesAsync(int receiptId, CancellationToken cancellationToken = default)
    {
        return await Context.Receipts
            .Include(r => r.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(r => r.Id == receiptId, cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetReceiptsWithLinesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Receipts
            .Include(r => r.Lines)
            .ThenInclude(l => l.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}