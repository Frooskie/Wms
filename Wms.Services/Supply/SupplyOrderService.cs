using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.Supply;

public class SupplyOrderService(
    ISupplyOrderRepository orderRepository,
    IBatchRepository batchRepository,
    IReservationRepository reservationRepository,
    IInventoryTransactionRepository transactionRepository,
    ICellRepository cellRepository,
    IRepository<SupplyOrderLine>? lineRepository,
    IProductRepository productRepository)
    : BaseService<SupplyOrder>(orderRepository), ISupplyOrderService
{
    public async Task<SupplyOrder> CreateOrderAsync(SupplyOrder order, List<SupplyOrderLine> lines, CancellationToken cancellationToken = default)
    {
        foreach (var line in lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), line.ProductId);
        }

        order.Lines = lines;
        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);
        return order;
    }
    
    public async Task<SupplyOrder?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(id, cancellationToken);
    
    public async Task<IEnumerable<SupplyOrder>> GetAllWithLinesAsync(CancellationToken cancellationToken = default)
        => await orderRepository.GetSupplyOrdersWithLinesAsync(cancellationToken);
    
    public async Task ConfirmOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(SupplyOrder), orderId);

        if (order.Status != SupplyOrderStatus.Draft)
            throw new BusinessRuleException("Only draft orders can be confirmed.");

        var reservationsToCreate = new List<Reservation>();
        var batchesToUpdate = new List<Batch>();

        foreach (var line in order.Lines)
        {
            var productId = line.ProductId;
            var requestedQty = line.Quantity;
            
            var availableBatches = await batchRepository.FindAsync(
                b => b.ProductId == productId && (b.Quantity - b.ReservedQuantity) > 0,
                cancellationToken);
            
            var sortedBatches = availableBatches.OrderBy(b => b.ExpiryDate).ToList();

            var remaining = requestedQty;
            foreach (var batch in sortedBatches)
            {
                if (remaining <= 0) break;
                var available = batch.Quantity - batch.ReservedQuantity;
                var reserveQty = Math.Min(available, remaining);
                
                if (reserveQty > 0)
                {
                    var reservation = new Reservation
                    {
                        BatchId = batch.Id,
                        SupplyOrderId = order.Id,
                        Quantity = reserveQty
                    };
                    reservationsToCreate.Add(reservation);

                    batch.ReservedQuantity += reserveQty;
                    batchesToUpdate.Add(batch);

                    remaining -= reserveQty;
                }
            }

            if (remaining > 0)
            {
                throw new BusinessRuleException(
                    $"Not enough stock for product ID {productId}. Missing {remaining} units.");
            }
        }
        
        foreach (var res in reservationsToCreate)
            await reservationRepository.AddAsync(res, cancellationToken);

        foreach (var batch in batchesToUpdate)
            batchRepository.Update(batch);

        order.Status = SupplyOrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        orderRepository.Update(order);

        await orderRepository.SaveChangesAsync(cancellationToken);
    }
    
    public async Task ShipOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(SupplyOrder), orderId);

        if (order.Status != SupplyOrderStatus.Confirmed)
            throw new BusinessRuleException("Only confirmed orders can be shipped.");

        var reservations = order.Reservations.ToList();
        if (reservations.Count == 0)
            throw new BusinessRuleException("No reservations found for this order.");
        
        var grouped = reservations
            .GroupBy(r => r.BatchId)
            .Select(g => new
            {
                BatchId = g.Key,
                TotalReserved = g.Sum(r => r.Quantity),
                Reservations = g.ToList()
            })
            .ToList();

        var transactions = new List<InventoryTransaction>();

        foreach (var group in grouped)
        {
            var batch = await batchRepository.GetByIdAsync(group.BatchId, cancellationToken);
            if (batch == null)
                throw new NotFoundException(nameof(Batch), group.BatchId);
            
            batch.Quantity -= group.TotalReserved;
            batch.ReservedQuantity -= group.TotalReserved;
            
            if (batch.Quantity == 0)
            {
                var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
                if (cell != null)
                {
                    cell.IsOccupied = false;
                    cellRepository.Update(cell);
                }
            }

            batchRepository.Update(batch);
            
            var transaction = new InventoryTransaction
            {
                BatchId = batch.Id,
                QuantityChange = -group.TotalReserved,
                TransactionType = TransactionType.Out,
                DocumentId = orderId,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };
            transactions.Add(transaction);
        }
        
        foreach (var res in reservations)
            reservationRepository.Delete(res);
        
        foreach (var t in transactions)
            await transactionRepository.AddAsync(t, cancellationToken);
        
        order.Status = SupplyOrderStatus.Shipped;
        order.ShippedAt = DateTime.UtcNow;
        orderRepository.Update(order);
        
        await orderRepository.SaveChangesAsync(cancellationToken);
    }
}