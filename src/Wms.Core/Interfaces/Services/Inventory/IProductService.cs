using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Core.Interfaces.Services.Inventory;

public interface IProductService : ICrudService<Product>
{
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category,
        CancellationToken cancellationToken = default);
}