using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : Repository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name,
        CancellationToken cancellationToken = default)
    {
        return await Context.Products
            .Where(p => p.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category,
        CancellationToken cancellationToken = default)
    {
        return await Context.Products
            .Where(p => p.Category == category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}