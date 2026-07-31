using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.ProductService;

public class ProductService(IRepository<Product> repository, IProductRepository productRepository)
    : CrudService<Product>(repository), IProductService
{
    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await productRepository.GetProductsByNameAsync(name, cancellationToken);

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
        => await productRepository.GetProductsByCategoryAsync(category, cancellationToken);
}