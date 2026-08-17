namespace Wms.API.DTOs.WarehouseStructure;

public record CreateShelfRequest
{
    /// <summary>Номер полки.</summary>
    public int Number { get; init; }

    public int RackId { get; init; }
}