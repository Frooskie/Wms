using Wms.Core.DTOs;
using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category,
        CancellationToken cancellationToken = default);
}