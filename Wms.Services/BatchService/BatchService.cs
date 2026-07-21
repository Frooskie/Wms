using Wms.Core.Entities;
//using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.BatchService;

public class BatchService : BaseService<Batch>, IBatchService
{
    private readonly IBatchRepository _batchRepository;
    private readonly ICellRepository _cellRepository;

    public BatchService(IBatchRepository batchRepository, ICellRepository cellRepository)
        : base(batchRepository)
    {
        _batchRepository = batchRepository;
        _cellRepository = cellRepository;
    }

    public override async Task<IEnumerable<Batch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetBatchesWithDetailsAsync(cancellationToken);
    }

    public override async Task<Batch?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetBatchWithProductAndCellAsync(id, cancellationToken);
    }

    public override async Task<Batch> CreateAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        var isOccupied = await _batchRepository.IsCellOccupiedByOtherBatchAsync(batch.CellId, null, cancellationToken);
        if (isOccupied)
            throw new InvalidOperationException("Cell is already occupied by another batch.");

        var cell = await _cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell == null)
            throw new InvalidOperationException("Cell not found.");
        
        cell.IsOccupied = true;
        _cellRepository.Update(cell);
        
        return await base.CreateAsync(batch, cancellationToken);
    }

    public override async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(id, cancellationToken);
        if (batch == null)
            return;
        
        var cell = await _cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (cell != null)
        {
            cell.IsOccupied = false;
            _cellRepository.Update(cell);
        }
        
        await base.DeleteAsync(id, cancellationToken);
    }

    public async Task MoveBatchAsync(int batchId, int newCellId, CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetBatchWithProductAndCellAsync(batchId, cancellationToken);
        if (batch == null)
            // throw new NotFoundException($"Batch with id {batchId} not found.");
        throw new InvalidOperationException($"Batch with id {batchId} not found.");

        var newCell = await _cellRepository.GetByIdAsync(newCellId, cancellationToken);
        if (newCell == null)
            throw new InvalidOperationException("Target cell not found.");
        
        var isOccupied = await _batchRepository.IsCellOccupiedByOtherBatchAsync(newCellId, batchId, cancellationToken);
        if (isOccupied)
            throw new InvalidOperationException("Target cell is already occupied.");
        
        var oldCell = await _cellRepository.GetByIdAsync(batch.CellId, cancellationToken);
        if (oldCell != null)
        {
            oldCell.IsOccupied = false;
            _cellRepository.Update(oldCell);
        }
        
        newCell.IsOccupied = true;
        _cellRepository.Update(newCell);
        
        batch.CellId = newCellId;
        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesWithFiltersAsync(
        int? productId,
        int? cellId,
        DateTime? expiryFrom,
        DateTime? expiryTo,
        CancellationToken cancellationToken = default)
    {
        var batches = await _batchRepository.GetBatchesWithDetailsAsync(cancellationToken);
        
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