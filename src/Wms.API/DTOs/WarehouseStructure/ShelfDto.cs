namespace Wms.API.DTOs.WarehouseStructure;

public record ShelfDto
{
    public int Id { get; init; }
    public int Number { get; init; }
    public int RackId { get; init; }
    public List<CellDto> Cells { get; init; } = [];
}