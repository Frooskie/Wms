using Wms.Core.Entities;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.BatchService;

public class BatchService(IBatchRepository batchRepository, ICellRepository cellRepository)
    : ReadOnlyService<Batch>(batchRepository), IBatchService
{
    public async Task<Batch> CreateBatchAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        var isOccupied = await batchRepository.IsCellOccupiedByOtherBatchAsync(batch.CellId, null, cancellationToken);

        if (isOccupied)
            throw new BusinessRuleException("Cell is already occupied by another batch.");

        var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell == null)
            throw new NotFoundException("Cell not found.");

        cell.IsOccupied = true;
        cellRepository.Update(cell);

        await _repository.AddAsync(batch, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return batch;
    }

    public async Task DeleteBatchAsync(int id, CancellationToken cancellationToken = default)
    {
        var batch = await batchRepository.GetByIdAsync(id, cancellationToken);
        if (batch == null)
            return;

        var cell = await cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell != null)
        {
            cell.IsOccupied = false;
            cellRepository.Update(cell);
        }

        _repository.Delete(batch);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveBatchAsync(int batchId, int newCellId, CancellationToken cancellationToken = default)
    {
        var batch = await batchRepository.GetBatchWithProductAndCellAsync(batchId, cancellationToken);
        if (batch == null)
            throw new NotFoundException($"Batch with id {batchId} not found.");

        var newCell = await cellRepository.GetByIdAsync(newCellId, cancellationToken);
        if (newCell == null)
            throw new NotFoundException("Target cell not found.");

        var isOccupied = await batchRepository.IsCellOccupiedByOtherBatchAsync(newCellId, batchId, cancellationToken);
        if (isOccupied)
            throw new BusinessRuleException("Target cell is already occupied.");

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
        await batchRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesWithFiltersAsync(
        int? productId,
        int? cellId,
        DateTime? expiryFrom,
        DateTime? expiryTo,
        CancellationToken cancellationToken = default)
    {
        var batches = await batchRepository.GetBatchesWithDetailsAsync(cancellationToken);

        if (productId.HasValue)
            batches = batches.Where(b => b.ProductId == productId.Value);
        if (cellId.HasValue)
            batches = batches.Where(b => b.CellId == cellId.Value);
        if (expiryFrom.HasValue)
            batches = batches.Where(b => b.ExpiryDate >= expiryFrom.Value);
        if (expiryTo.HasValue)
            batches = batches.Where(b => b.ExpiryDate <= expiryTo.Value);

        return batches;
    }
}