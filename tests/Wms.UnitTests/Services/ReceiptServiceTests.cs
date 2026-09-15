using FluentAssertions;
using Moq;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Inventory;
using Wms.Services.Documents;

namespace Wms.UnitTests.Services;

public class ReceiptServiceTests
{
    private readonly Mock<IReceiptRepository> _receiptRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICellRepository> _cellRepositoryMock;
    private readonly Mock<IBatchService> _batchServiceMock;
    private readonly ReceiptService _receiptService;

    public ReceiptServiceTests()
    {
        _receiptRepositoryMock = new Mock<IReceiptRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _cellRepositoryMock = new Mock<ICellRepository>();
        _batchServiceMock = new Mock<IBatchService>();
        _receiptService = new ReceiptService(
            _receiptRepositoryMock.Object,
            _productRepositoryMock.Object,
            _cellRepositoryMock.Object,
            _batchServiceMock.Object);
    }

    [Fact]
    public async Task ReceiveReceiptAsync_ShouldCreateBatchesAndSetComment_WhenDiscrepancyExists()
    {
        const int receiptId = 1;
        const string userId = "user123";
        const int productId = 100;
        const int expectedQty = 50;
        const int actualQty = 45;
        const int cellId = 10;
        var expiryDate = DateTime.UtcNow.AddDays(30);
        const decimal purchasePrice = 10.5m;

        var receipt = new Receipt
        {
            Id = receiptId,
            Status = ReceiptStatus.Pending,
            Lines = new List<ReceiptLine>
            {
                new() { ProductId = productId, ExpectedQuantity = expectedQty }
            }
        };

        var cell = new Cell { Id = cellId, IsOccupied = false, Code = "A1" };

        _receiptRepositoryMock
            .Setup(r => r.GetReceiptWithLinesAsync(receiptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receipt);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(cellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cell);

        _batchServiceMock
            .Setup(b => b.CreateBatchAsync(It.IsAny<Batch>(), userId, receiptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Batch b, string u, int? docId, CancellationToken ct) => b);

        var receiveLines =
            new List<(int productId, int actualQuantity, int cellId, DateTime expiryDate, decimal purchasePrice)>
            {
                (productId, actualQty, cellId, expiryDate, purchasePrice)
            };

        // Act
        await _receiptService.ReceiveReceiptAsync(receiptId, receiveLines, userId, CancellationToken.None);

        // Assert
        receipt.Status.Should().Be(ReceiptStatus.Received);
        receipt.Comment.Should().Contain(
            string.Format(ErrorMessages.Receipt.DiscrepancyQuantity, productId, expectedQty, actualQty));

        _batchServiceMock.Verify(bs => bs.CreateBatchAsync(
                It.Is<Batch>(b => b.Quantity == actualQty && b.CellId == cellId),
                userId,
                receiptId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _receiptRepositoryMock.Verify(r => r.Update(receipt), Times.Once);
        _receiptRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReceiveReceiptAsync_ShouldThrowBusinessRuleException_WhenCellIsOccupied()
    {
        const int receiptId = 1;
        const string userId = "user123";
        const int productId = 100;
        const int cellId = 10;

        var receipt = new Receipt
        {
            Id = receiptId,
            Status = ReceiptStatus.Pending,
            Lines = new List<ReceiptLine>()
        };

        var cell = new Cell { Id = cellId, IsOccupied = true, Code = "A1" };

        _receiptRepositoryMock
            .Setup(r => r.GetReceiptWithLinesAsync(receiptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receipt);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(cellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cell);

        var receiveLines =
            new List<(int productId, int actualQuantity, int cellId, DateTime expiryDate, decimal purchasePrice)>
            {
                (productId, 10, cellId, DateTime.UtcNow.AddDays(30), 10.0m)
            };

        // Act
        var act = async () =>
            await _receiptService.ReceiveReceiptAsync(receiptId, receiveLines, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .Where(ex => ex.Code == ErrorCodes.CellOccupied)
            .WithMessage(ErrorMessages.Receipt.CellOccupied(cell.Code));
    }

    [Fact]
    public async Task ReceiveReceiptAsync_ShouldThrowNotFoundException_WhenReceiptNotFound()
    {
        const int receiptId = 999;
        const string userId = "user123";

        _receiptRepositoryMock
            .Setup(r => r.GetReceiptWithLinesAsync(receiptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receipt?)null);

        var receiveLines =
            new List<(int productId, int actualQuantity, int cellId, DateTime expiryDate, decimal purchasePrice)>();

        // Act
        var act = async () =>
            await _receiptService.ReceiveReceiptAsync(receiptId, receiveLines, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .Where(ex => ex.Code == ErrorCodes.NotFound)
            .WithMessage(ErrorMessages.Receipt.NotFoundFormat(receiptId));
    }
}