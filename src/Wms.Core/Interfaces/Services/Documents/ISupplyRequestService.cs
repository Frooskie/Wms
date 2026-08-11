using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services.Documents;

public interface ISupplyRequestService
{
    Task<SupplyRequest> CreateSupplyRequestAsync(SupplyRequest request, List<SupplyRequestLine> lines,
        CancellationToken cancellationToken = default);

    Task<SupplyRequest?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyRequest>> GetAllWithLinesAsync(CancellationToken cancellationToken = default);
    Task SubmitAsync(int id, string currentUserId, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task RejectAsync(int id, CancellationToken cancellationToken = default);
}