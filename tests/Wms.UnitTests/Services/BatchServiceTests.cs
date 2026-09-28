using FluentAssertions;
using Moq;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Audit;
using Wms.Services.Inventory;

namespace Wms.UnitTests.Services;

public class BatchServiceTests
{
    private readonly Mock<IBatchRepository> _batchRepositoryMock;
    private readonly Mock<ICellRepository> _cellRepositoryMock;
    private readonly Mock<IInventoryTransactionService> _transactionServiceMock;
    private readonly BatchService _batchService;

    public BatchServiceTests()
    {
        _batchRepositoryMock = new Mock<IBatchRepository>();
        _cellRepositoryMock = new Mock<ICellRepository>();
        _transactionServiceMock = new Mock<IInventoryTransactionService>();
        _batchService = new BatchService(
            _batchRepositoryMock.Object,
            _cellRepositoryMock.Object,
            _transactionServiceMock.Object);
    }

    [Fact]
    public async Task MoveBatchAsync_ShouldMoveBatch_WhenTargetCellIsFree()
    {
        const int batchId = 1;
        const int newCellId = 2;
        const string userId = "user123";

        var oldCell = new Cell { Id = 10, IsOccupied = true };
        var newCell = new Cell { Id = newCellId, IsOccupied = false };
        var batch = new Batch
        {
            Id = batchId,
            CellId = oldCell.Id,
            Quantity = 100,
            ReservedQuantity = 0
        };

        _batchRepositoryMock
            .Setup(r => r.GetBatchWithProductAndCellAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(newCellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCell);

        _batchRepositoryMock
            .Setup(r => r.IsCellOccupiedByOtherBatchAsync(newCellId, batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(oldCell.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldCell);

        // Act
        await _batchService.MoveBatchAsync(batchId, newCellId, userId, CancellationToken.None);

        // Assert
        oldCell.IsOccupied.Should().BeFalse();
        newCell.IsOccupied.Should().BeTrue();
        batch.CellId.Should().Be(newCellId);

        _cellRepositoryMock.Verify(r => r.Update(oldCell), Times.Once);
        _cellRepositoryMock.Verify(r => r.Update(newCell), Times.Once);
        _batchRepositoryMock.Verify(r => r.Update(batch), Times.Once);

        _transactionServiceMock.Verify(
            t => t.AddTransactionAsync(
                batch, 0, TransactionType.Move, userId, null, oldCell.Id, newCell.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MoveBatchAsync_ShouldThrowBusinessRuleException_WhenTargetCellIsOccupied()
    {
        const int batchId = 1;
        const int newCellId = 2;
        const string userId = "user123";

        var batch = new Batch { Id = batchId, CellId = 10 };
        var newCell = new Cell { Id = newCellId, IsOccupied = true };

        _batchRepositoryMock
            .Setup(r => r.GetBatchWithProductAndCellAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(newCellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCell);

        _batchRepositoryMock
            .Setup(r => r.IsCellOccupiedByOtherBatchAsync(newCellId, batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () =>
            await _batchService.MoveBatchAsync(batchId, newCellId, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .Where(ex => ex.Code == ErrorCodes.TargetCellOccupied)
            .WithMessage(ErrorMessages.Batch.TargetCellOccupied);
    }

    [Fact]
    public async Task MoveBatchAsync_ShouldThrowNotFoundException_WhenBatchDoesNotExist()
    {
        const int batchId = 999;
        const int newCellId = 2;
        const string userId = "user123";

        _batchRepositoryMock
            .Setup(r => r.GetBatchWithProductAndCellAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Batch?)null);

        // Act
        var act = async () =>
            await _batchService.MoveBatchAsync(batchId, newCellId, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .Where(ex => ex.Code == ErrorCodes.NotFound)
            .WithMessage(ErrorMessages.Batch.NotFoundFormat(batchId));
    }

    [Fact]
    public async Task MoveBatchAsync_ShouldThrowNotFoundException_WhenTargetCellDoesNotExist()
    {
        const int batchId = 1;
        const int newCellId = 999;
        const string userId = "user123";

        var batch = new Batch { Id = batchId, CellId = 10 };

        _batchRepositoryMock
            .Setup(r => r.GetBatchWithProductAndCellAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _cellRepositoryMock
            .Setup(r => r.GetByIdAsync(newCellId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cell?)null);

        // Act
        var act = async () =>
            await _batchService.MoveBatchAsync(batchId, newCellId, userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .Where(ex => ex.Code == ErrorCodes.NotFound)
            .WithMessage(ErrorMessages.Batch.TargetCellNotFound);
    }
}