using Wms.Core.Constants;
using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Audit;
using Wms.Core.Interfaces.Services.Inventory;
using Wms.Services.Base;

namespace Wms.Services.Inventory;

public class BatchService(
    IBatchRepository batchRepository,
    ICellRepository cellRepository,
    IInventoryTransactionService transactionService)
    : ReadOnlyService<Batch>(batchRepository), IBatchService
{
    public async Task<Batch> CreateBatchAsync(
        Batch batch,
        string userId,
        int? documentId = null,
        CancellationToken cancellationToken = default)
    {
        var isOccupied = await batchRepository.IsCellOccupiedByOtherBatchAsync(batch.CellId, null, cancellationToken);
        if (isOccupied)
            throw new BusinessRuleException(ErrorMessages.Batch.CellOccupied, ErrorCodes.CellOccupied);

        var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell == null)
            throw new NotFoundException(ErrorMessages.Batch.CellNotFound);

        cell.IsOccupied = true;
        cellRepository.Update(cell);

        await _repository.AddAsync(batch, cancellationToken);

        await transactionService.AddTransactionAsync(
            batch.Id,
            batch.Quantity,
            TransactionType.In,
            userId,
            documentId,
            cancellationToken: cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return batch;
    }

    public async Task DeleteBatchAsync(int id, CancellationToken cancellationToken = default)
    {
        var batch = await batchRepository.GetByIdAsync(id, cancellationToken);
        if (batch == null)
            throw new NotFoundException(ErrorMessages.Batch.NotFoundFormat(id));

        var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell != null)
        {
            cell.IsOccupied = false;
            cellRepository.Update(cell);
        }

        _repository.Delete(batch);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveBatchAsync(
        int batchId,
        int newCellId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var batch = await batchRepository.GetBatchWithProductAndCellAsync(batchId, cancellationToken);
        if (batch == null)
            throw new NotFoundException(ErrorMessages.Batch.NotFoundFormat(batchId));

        var newCell = await cellRepository.GetByIdAsync(newCellId, cancellationToken);
        if (newCell == null)
            throw new NotFoundException(ErrorMessages.Batch.TargetCellNotFound);

        var isOccupied = await batchRepository.IsCellOccupiedByOtherBatchAsync(newCellId, batchId, cancellationToken);
        if (isOccupied)
            throw new BusinessRuleException(ErrorMessages.Batch.TargetCellOccupied, ErrorCodes.TargetCellOccupied);

        var oldCell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (oldCell != null)
        {
            oldCell.IsOccupied = false;
            cellRepository.Update(oldCell);
        }

        newCell.IsOccupied = true;
        cellRepository.Update(newCell);

        batch.CellId = newCellId;
        batchRepository.Update(batch);

        await transactionService.AddTransactionAsync(
            batch.Id,
            0,
            TransactionType.Move,
            userId,
            null, // документ на перемещение не предусмотрен
            oldCell?.Id,
            newCell.Id,
            cancellationToken);

        await batchRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Batch>> GetPagedBatchesWithFiltersAsync(
        int? productId = null,
        int? cellId = null,
        DateTime? expiryFrom = null,
        DateTime? expiryTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await batchRepository.GetBatchesPagedFilteredAsync(
            productId, cellId, expiryFrom, expiryTo, pageNumber, pageSize, cancellationToken);
    }
}