using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.RackService;

public class RackService(IRepository<Rack> repository, IRackRepository rackRepository)
    : CrudService<Rack>(repository), IRackService
{
    public async Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default)
        => await rackRepository.GetRacksWithShelvesAndCellsAsync(cancellationToken);

    public async Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default)
        => await rackRepository.GetRackWithShelvesAsync(rackId, cancellationToken);
}