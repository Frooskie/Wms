using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IZoneRepository : IRepository<Zone>
{
    Task<IEnumerable<Zone>> GetZonesWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Zone?> GetZoneWithRacksAsync(int zoneId, CancellationToken cancellationToken = default);
}

public interface IRackRepository : IRepository<Rack>
{
    Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default);
    Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default);
}

public interface ICellRepository : IRepository<Cell>
{
    Task<Cell?> GetCellWithShelfAsync(int cellId, CancellationToken cancellationToken = default);
    Task<bool> IsCellOccupiedAsync(int cellId, CancellationToken cancellationToken = default);
}

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default);
}