namespace Wms.API.DTOs.WarehouseStructure;

public class ShelfDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int RackId { get; set; }
    public List<CellDto> Cells { get; set; } = [];
}