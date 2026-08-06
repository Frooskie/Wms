using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;

namespace Wms.Services.ReceiptService;

public class ReceiptService(
    IReceiptRepository receiptRepository,
    IRepository<ReceiptLine> receiptLineRepository,
    IProductRepository productRepository,
    ICellRepository cellRepository,
    IBatchService batchService)
    : IReceiptService
{
    public async Task<Receipt> CreateReceiptAsync(Receipt receipt, List<ReceiptLine> lines,
        CancellationToken cancellationToken = default)
    {
        foreach (var line in lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with id {line.ProductId} not found.");
        }

        receipt.Lines = lines;
        await receiptRepository.AddAsync(receipt, cancellationToken);
        await receiptRepository.SaveChangesAsync(cancellationToken);
        return receipt;
    }

    public async Task<Receipt?> GetReceiptWithLinesAsync(int receiptId, CancellationToken cancellationToken = default)
    {
        return await receiptRepository.GetReceiptWithLinesAsync(receiptId, cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetAllReceiptsWithLinesAsync(CancellationToken cancellationToken = default)
    {
        return await receiptRepository.GetReceiptsWithLinesAsync(cancellationToken);
    }

    public async Task ReceiveReceiptAsync(int receiptId,
        List<(int productId, int actualQuantity, int cellId, DateTime expiryDate, decimal purchasePrice)> receiveLines,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var receipt = await receiptRepository.GetReceiptWithLinesAsync(receiptId, cancellationToken);

        if (receipt == null)
            throw new NotFoundException($"Receipt with id {receiptId} not found.");
        if (receipt.Status != ReceiptStatus.Pending)
            throw new InvalidOperationException($"Receipt is already {receipt.Status}.");

        foreach (var line in receiveLines)
        {
            var cell = await cellRepository.GetByIdAsync(line.cellId, cancellationToken);

            if (cell == null)
                throw new InvalidOperationException($"Cell with id {line.cellId} not found.");
            if (cell.IsOccupied)
                throw new InvalidOperationException($"Cell {cell.Code} is already occupied.");
        }

        foreach (var line in receiveLines)
        {
            var batch = new Batch
            {
                ProductId = line.productId,
                Quantity = line.actualQuantity,
                ReservedQuantity = 0,
                PurchasePrice = line.purchasePrice,
                ProductionDate = DateTime.UtcNow,
                ExpiryDate = line.expiryDate,
                ReceivedDate = DateTime.UtcNow,
                CellId = line.cellId
            };

            await batchService.CreateBatchAsync(batch, userId, receiptId, cancellationToken);
        }

        // Проверка расхождений
        var discrepancies = new List<string>();
        foreach (var expectedLine in receipt.Lines)
        {
            var actualLine = receiveLines.FirstOrDefault(l => l.productId == expectedLine.ProductId);
            if (actualLine == default)
                discrepancies.Add(
                    $"Product {expectedLine.ProductId}: expected {expectedLine.ExpectedQuantity}, but actual missing.");
            else if (actualLine.actualQuantity != expectedLine.ExpectedQuantity)
                discrepancies.Add(
                    $"Product {expectedLine.ProductId}: expected {expectedLine.ExpectedQuantity}, actual {actualLine.actualQuantity}.");
        }

        if (discrepancies.Count != 0) receipt.Comment = string.Join("; ", discrepancies);

        receipt.Status = ReceiptStatus.Received;
        receiptRepository.Update(receipt);

        await receiptRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectReceiptAsync(int receiptId, CancellationToken cancellationToken = default)
    {
        var receipt = await receiptRepository.GetByIdAsync(receiptId, cancellationToken);

        if (receipt == null)
            throw new NotFoundException($"Receipt with id {receiptId} not found.");
        if (receipt.Status != ReceiptStatus.Pending)
            throw new InvalidOperationException($"Receipt is already {receipt.Status}.");

        receipt.Status = ReceiptStatus.Rejected;
        receiptRepository.Update(receipt);

        await receiptRepository.SaveChangesAsync(cancellationToken);
    }
}