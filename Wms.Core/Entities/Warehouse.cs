namespace Wms.Core.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
    
    public ICollection<Zone> Zones { get; set; } = new List<Zone>();
}