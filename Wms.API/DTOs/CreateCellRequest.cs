namespace Wms.API.DTOs;

public class CreateCellRequest
{
    public string Code { get; set; } = string.Empty; // например, "A-1-1"
    public int ShelfId { get; set; }
}