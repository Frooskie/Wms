using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.Documents;
using Wms.Core.Interfaces.Services.Inventory;

namespace Wms.Services.Documents;

public class ReceiptService(
    IReceiptRepository receiptRepository,
    IProductRepository productRepository,
    ICellRepository cellRepository,
    IBatchService batchService)
    : IReceiptService
{
    public async Task<Receipt> CreateReceiptAsync(
        Receipt receipt,
        List<ReceiptLine> lines,
        CancellationToken cancellationToken = default)
    {
        foreach (var line in lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(ErrorMessages.Product.NotFoundFormat(line.ProductId));
        }

        receipt.Lines = lines;
        await receiptRepository.AddAsync(receipt, cancellationToken);
        await receiptRepository.SaveChangesAsync(cancellationToken);

        return receipt;
    }

    public async Task<Receipt?> GetReceiptWithLinesAsync(
        int receiptId,
        CancellationToken cancellationToken = default)
    {
        return await receiptRepository.GetReceiptWithLinesAsync(receiptId, cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetAllReceiptsWithLinesAsync(
        CancellationToken cancellationToken = default)
    {
        return await receiptRepository.GetReceiptsWithLinesAsync(cancellationToken);
    }

    public async Task ReceiveReceiptAsync(
        int receiptId,
        List<(int productId, int actualQuantity, int cellId, DateOnly expiryDate, decimal purchasePrice)> receiveLines,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var receipt = await receiptRepository.GetReceiptWithLinesAsync(receiptId, cancellationToken);

        if (receipt == null)
            throw new NotFoundException(ErrorMessages.Receipt.NotFoundFormat(receiptId));

        if (receipt.Status != ReceiptStatus.Pending)
            throw new BusinessRuleException(
                ErrorMessages.Receipt.AlreadyProcessedFormat(receipt.Status.ToString()),
                ErrorCodes.ReceiptAlreadyProcessed);

        foreach (var line in receiveLines)
        {
            var cell = await cellRepository.GetByIdAsync(line.cellId, cancellationToken);

            if (cell == null)
                throw new NotFoundException(ErrorMessages.Receipt.CellNotFound(line.cellId));

            if (cell.IsOccupied)
                throw new BusinessRuleException(ErrorMessages.Receipt.CellOccupied(cell.Code), ErrorCodes.CellOccupied);
        }
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var line in receiveLines)
        {
            var batch = new Batch
            {
                ProductId = line.productId,
                Quantity = line.actualQuantity,
                ReservedQuantity = 0,
                PurchasePrice = line.purchasePrice,
                ProductionDate = today,
                ExpiryDate = line.expiryDate,
                ReceivedDate = today,
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
                discrepancies.Add(string.Format(
                    ErrorMessages.Receipt.DiscrepancyMissing,
                    expectedLine.ProductId,
                    expectedLine.ExpectedQuantity));
            else if (actualLine.actualQuantity != expectedLine.ExpectedQuantity)
                discrepancies.Add(string.Format(
                    ErrorMessages.Receipt.DiscrepancyQuantity,
                    expectedLine.ProductId,
                    expectedLine.ExpectedQuantity,
                    actualLine.actualQuantity));
        }

        if (discrepancies.Count != 0)
            receipt.Comment = string.Join("; ", discrepancies);

        receipt.Status = ReceiptStatus.Received;
        receiptRepository.Update(receipt);

        await receiptRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectReceiptAsync(int receiptId, CancellationToken cancellationToken = default)
    {
        var receipt = await receiptRepository.GetByIdAsync(receiptId, cancellationToken);

        if (receipt == null)
            throw new NotFoundException(ErrorMessages.Receipt.NotFoundFormat(receiptId));

        if (receipt.Status != ReceiptStatus.Pending)
            throw new BusinessRuleException(
                ErrorMessages.Receipt.AlreadyProcessedFormat(receipt.Status.ToString()),
                ErrorCodes.ReceiptAlreadyProcessed);

        receipt.Status = ReceiptStatus.Rejected;
        receiptRepository.Update(receipt);

        await receiptRepository.SaveChangesAsync(cancellationToken);
    }
}