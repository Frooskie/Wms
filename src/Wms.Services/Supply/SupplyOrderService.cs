using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;

namespace Wms.Services.Supply;

public class SupplyOrderService(
    ISupplyOrderRepository orderRepository,
    IRepository<SupplyOrderLine> orderLineRepository,
    IBatchRepository batchRepository,
    IReservationRepository reservationRepository,
    IInventoryTransactionRepository transactionRepository,
    ICellRepository cellRepository,
    IProductRepository productRepository)
    : ISupplyOrderService
{
    public async Task<SupplyOrder> CreateOrderAsync(
        SupplyOrder order,
        List<SupplyOrderLine> lines,
        CancellationToken cancellationToken = default)
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

    public async Task<SupplyOrder?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<SupplyOrder>> GetAllWithLinesAsync(
        CancellationToken cancellationToken = default)
    {
        return await orderRepository.GetSupplyOrdersWithLinesAsync(cancellationToken);
    }

    public async Task ConfirmOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(SupplyOrder), orderId);

        if (order.Status != SupplyOrderStatus.Draft)
            throw new BusinessRuleException(
                $"Cannot confirm order with status '{order.Status}'. Only Draft orders can be confirmed.");

        var reservationsToCreate = new List<Reservation>();
        var batchesToUpdate = new List<Batch>();

        foreach (var line in order.Lines)
        {
            var productId = line.ProductId;
            var requestedQty = line.Quantity;

            // Находим все партии для данного продукта, у которых есть доступный остаток
            var availableBatches = await batchRepository.FindAsync(
                b => b.ProductId == productId && b.Quantity - b.ReservedQuantity > 0,
                cancellationToken);

            // Сортируем по сроку годности (FIFO)
            var sortedBatches = availableBatches.OrderBy(b => b.ExpiryDate).ToList();

            var remainingToReserve = requestedQty;

            foreach (var batch in sortedBatches)
            {
                if (remainingToReserve <= 0)
                    break;

                var available = batch.Quantity - batch.ReservedQuantity;
                var reserveQty = Math.Min(available, remainingToReserve);

                if (reserveQty > 0)
                {
                    var reservation = new Reservation
                    {
                        BatchId = batch.Id,
                        SupplyOrderId = order.Id,
                        Quantity = reserveQty
                    };
                    reservationsToCreate.Add(reservation);

                    // Обновляем зарезервированное количество в партии
                    batch.ReservedQuantity += reserveQty;
                    batchesToUpdate.Add(batch);

                    remainingToReserve -= reserveQty;
                }
            }

            if (remainingToReserve > 0)
                throw new BusinessRuleException(
                    $"Not enough available stock for product ID {productId}. Missing {remainingToReserve} units.");
        }

        foreach (var reservation in reservationsToCreate)
            await reservationRepository.AddAsync(reservation, cancellationToken);

        foreach (var batch in batchesToUpdate)
            batchRepository.Update(batch);

        order.Status = SupplyOrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        orderRepository.Update(order);

        await orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ShipOrderAsync(
        int orderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetSupplyOrderWithLinesAndReservationsAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(SupplyOrder), orderId);

        if (order.Status != SupplyOrderStatus.Confirmed)
            throw new BusinessRuleException(
                $"Cannot ship order with status '{order.Status}'. Only Confirmed orders can be shipped.");

        var reservations = order.Reservations.ToList();
        if (reservations.Count == 0)
            throw new BusinessRuleException("Order has no reservations. Cannot ship.");

        // Группируем резервы по партии для оптимизации
        var groupedReservations = reservations
            .GroupBy(r => r.BatchId)
            .Select(g => new
            {
                BatchId = g.Key,
                TotalReserved = g.Sum(r => r.Quantity),
                Reservations = g.ToList()
            })
            .ToList();

        var transactions = new List<InventoryTransaction>();
        var batchesToUpdate = new List<Batch>();
        var cellsToUpdate = new List<Cell>();

        foreach (var group in groupedReservations)
        {
            var batch = await batchRepository.GetByIdAsync(group.BatchId, cancellationToken);
            if (batch == null)
                throw new NotFoundException(nameof(Batch), group.BatchId);

            // Списываем зарезервированное количество
            batch.Quantity -= group.TotalReserved;
            batch.ReservedQuantity -= group.TotalReserved;

            if (batch.Quantity == 0)
            {
                var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
                if (cell != null && cell.IsOccupied)
                {
                    cell.IsOccupied = false;
                    cellsToUpdate.Add(cell);
                }
            }

            batchesToUpdate.Add(batch);

            // Создаём транзакцию списания
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

        // Удаляем все резервы этого заказа
        foreach (var reservation in reservations)
            reservationRepository.Delete(reservation);

        foreach (var transaction in transactions)
            await transactionRepository.AddAsync(transaction, cancellationToken);

        foreach (var batch in batchesToUpdate)
            batchRepository.Update(batch);

        foreach (var cell in cellsToUpdate)
            cellRepository.Update(cell);

        order.Status = SupplyOrderStatus.Shipped;
        order.ShippedAt = DateTime.UtcNow;
        orderRepository.Update(order);

        await orderRepository.SaveChangesAsync(cancellationToken);
    }
}