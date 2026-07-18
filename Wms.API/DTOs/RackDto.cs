namespace Wms.API.DTOs;

public class RackDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public List<ShelfDto> Shelves { get; set; } = [];
}