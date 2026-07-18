namespace Wms.API.DTOs;

public class CellDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int ShelfId { get; set; }
    public bool IsOccupied { get; set; }
}