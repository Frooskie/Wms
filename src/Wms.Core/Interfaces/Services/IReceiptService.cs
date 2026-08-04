using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface IReceiptService
{
    Task<Receipt> CreateReceiptAsync(Receipt receipt, List<ReceiptLine> lines,
        CancellationToken cancellationToken = default);

    Task<Receipt?> GetReceiptWithLinesAsync(int receiptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Receipt>> GetAllReceiptsWithLinesAsync(CancellationToken cancellationToken = default);

    Task ReceiveReceiptAsync(int receiptId,
        List<(int productId, int actualQuantity, int cellId, DateTime expiryDate, decimal purchasePrice)> receiveLines,
        string userId, CancellationToken cancellationToken = default);

    Task RejectReceiptAsync(int receiptId, CancellationToken cancellationToken = default);
}