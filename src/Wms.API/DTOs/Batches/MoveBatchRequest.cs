namespace Wms.API.DTOs.Batches;

public record MoveBatchRequest
{
    public int CellId { get; init; }
}