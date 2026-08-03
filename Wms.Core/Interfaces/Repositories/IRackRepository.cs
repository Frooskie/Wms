using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IRackRepository : IRepository<Rack>
{
    Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default);
    Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default);
}