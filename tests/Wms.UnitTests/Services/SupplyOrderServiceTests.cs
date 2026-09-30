using FluentAssertions;
using Moq;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Audit;
using Wms.Services.Documents;

namespace Wms.UnitTests.Services;

public class SupplyOrderServiceTests
{
    private readonly Mock<ISupplyOrderRepository> _orderRepositoryMock;
    private readonly Mock<IRepository<SupplyOrderLine>> _orderLineRepositoryMock;
    private readonly Mock<IBatchRepository> _batchRepositoryMock;
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IInventoryTransactionService> _transactionServiceMock;
    private readonly Mock<ICellRepository> _cellRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly SupplyOrderService _supplyOrderService;

    public SupplyOrderServiceTests()
    {
        _orderRepositoryMock = new Mock<ISupplyOrderRepository>();
        _orderLineRepositoryMock = new Mock<IRepository<SupplyOrderLine>>();
        _batchRepositoryMock = new Mock<IBatchRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _transactionServiceMock = new Mock<IInventoryTransactionService>();
        _cellRepositoryMock = new Mock<ICellRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _supplyOrderService = new SupplyOrderService(
            _orderRepositoryMock.Object,
            _orderLineRepositoryMock.Object,
            _batchRepositoryMock.Object,
            _reservationRepositoryMock.Object,
            _transactionServiceMock.Object,
            _cellRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact]
    public async Task ConfirmOrderAsync_ShouldCreateReservations_WhenStockAvailable()
    {
        const int orderId = 1;
        const int productId = 100;
        const int requestedQty = 30;

        var order = new SupplyOrder
        {
            Id = orderId,
            Status = SupplyOrderStatus.Draft,
            Lines = new List<SupplyOrderLine>
            {
                new() { ProductId = productId, Quantity = requestedQty }
            }
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var batch1 = new Batch { Id = 1, ProductId = productId, Quantity = 20, ReservedQuantity = 0, ExpiryDate = today.AddDays(10) };
        var batch2 = new Batch { Id = 2, ProductId = productId, Quantity = 20, ReservedQuantity = 0, ExpiryDate = today.AddDays(20) };
        
        _orderRepositoryMock
            .Setup(r => r.GetSupplyOrderWithLinesAndReservationsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _batchRepositoryMock
            .Setup(r => r.FindAsync(
                It.Is<System.Linq.Expressions.Expression<Func<Batch, bool>>>(expr =>
                    expr.Compile().Invoke(new Batch { ProductId = productId, Quantity = 10, ReservedQuantity = 0 })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Batch> { batch1, batch2 });

        // Act
        await _supplyOrderService.ConfirmOrderAsync(orderId, CancellationToken.None);

        // Assert
        order.Status.Should().Be(SupplyOrderStatus.Confirmed);
        order.ConfirmedAt.Should().NotBeNull();

        batch1.ReservedQuantity.Should().Be(20);
        batch2.ReservedQuantity.Should().Be(10);

        _reservationRepositoryMock.Verify(r => r.AddAsync(
                It.Is<Reservation>(res => res.BatchId == 1 && res.Quantity == 20 && res.SupplyOrderId == orderId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _reservationRepositoryMock.Verify(r => r.AddAsync(
                It.Is<Reservation>(res => res.BatchId == 2 && res.Quantity == 10 && res.SupplyOrderId == orderId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _batchRepositoryMock.Verify(r => r.Update(batch1), Times.Once);
        _batchRepositoryMock.Verify(r => r.Update(batch2), Times.Once);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
        _orderRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmOrderAsync_ShouldThrowBusinessRuleException_WhenNotEnoughStock()
    {
        const int orderId = 1;
        const int productId = 100;
        const int requestedQty = 30;
        const int missingQty = 20;

        var order = new SupplyOrder
        {
            Id = orderId,
            Status = SupplyOrderStatus.Draft,
            Lines = new List<SupplyOrderLine>
            {
                new() { ProductId = productId, Quantity = requestedQty }
            }
        };

        var batch = new Batch { Id = 1, ProductId = productId, Quantity = 10, ReservedQuantity = 0 };

        _orderRepositoryMock
            .Setup(r => r.GetSupplyOrderWithLinesAndReservationsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _batchRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Batch, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Batch> { batch });

        // Act
        var act = async () => await _supplyOrderService.ConfirmOrderAsync(orderId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .Where(ex => ex.Code == ErrorCodes.NotEnoughStock)
            .WithMessage(ErrorMessages.SupplyOrder.NotEnoughStockFormat(productId, missingQty));
    }

    [Fact]
    public async Task ShipOrderAsync_ShouldDecreaseQuantitiesAndFreeCell_WhenSuccessful()
    {
        const int orderId = 1;
        const string userId = "user123";
        const int batchId = 10;
        const int cellId = 5;
        const int reservedQty = 100;

        var order = new SupplyOrder
        {
            Id = orderId,
            Status = SupplyOrderStatus.Confirmed,
            Reservations = new List<Reservation>
            {
                new() { Id = 1, BatchId = batchId, Quantity = reservedQty }
            }
        };

        var batch = new Batch
        {
            Id = batchId,
            Quantity = 100,
            ReservedQuantity = reservedQty,
            CellId = cellId
        };

        var cell = new Cell { Id = cellId, IsOccupied = true };

        _orderRepositoryMock
            .Setup(r => r.GetSupplyOrderWithLinesAndReservationsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _batchRepositoryMock
            .Setup(r => r.GetByIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(cellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cell);

        // Act
        await _supplyOrderService.ShipOrderAsync(orderId, userId, CancellationToken.None);

        // Assert
        batch.Quantity.Should().Be(100 - reservedQty);
        batch.ReservedQuantity.Should().Be(0);
        cell.IsOccupied.Should().BeFalse();

        _transactionServiceMock.Verify(t => t.AddTransactionAsync(
                batch,
                -reservedQty,
                TransactionType.Out,
                userId,
                orderId,
                null,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _reservationRepositoryMock.Verify(r => r.Delete(
                It.Is<Reservation>(res => res.Id == 1)),
            Times.Once);

        _batchRepositoryMock.Verify(r => r.Update(batch), Times.Once);
        _cellRepositoryMock.Verify(r => r.Update(cell), Times.Once);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
        _orderRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        order.Status.Should().Be(SupplyOrderStatus.Shipped);
        order.ShippedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShipOrderAsync_ShouldThrowBusinessRuleException_WhenOrderNotConfirmed()
    {
        const int orderId = 1;
        const string userId = "user123";
        var order = new SupplyOrder
        {
            Id = orderId,
            Status = SupplyOrderStatus.Draft
        };

        _orderRepositoryMock
            .Setup(r => r.GetSupplyOrderWithLinesAndReservationsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _supplyOrderService.ShipOrderAsync(orderId, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .Where(ex => ex.Code == ErrorCodes.OrderNotConfirmed)
            .WithMessage(ErrorMessages.SupplyOrder.OnlyConfirmedCanBeShipped(SupplyOrderStatus.Draft.ToString()));
    }
}