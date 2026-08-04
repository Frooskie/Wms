using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface IProductService : ICrudService<Product>
{
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category,
        CancellationToken cancellationToken = default);
}