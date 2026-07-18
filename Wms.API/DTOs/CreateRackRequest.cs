namespace Wms.API.DTOs;

public class CreateRackRequest
{
    public string Code { get; set; } = string.Empty; // например, "A"
    public int ZoneId { get; set; }
}