using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface ISupplyOrderService : IBaseService<SupplyOrder>
{
    Task<SupplyOrder> CreateOrderAsync(SupplyOrder order, List<SupplyOrderLine> lines, CancellationToken cancellationToken = default);
    Task<SupplyOrder?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyOrder>> GetAllWithLinesAsync(CancellationToken cancellationToken = default);
    Task ConfirmOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task ShipOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default);
}