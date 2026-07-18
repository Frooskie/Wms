using Wms.Core.Enums;

namespace Wms.Core.Entities;

public class Zone
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ZoneType Type { get; set; }
    public int WarehouseId { get; set; }
    
    // Навигационные свойства
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<Rack> Racks { get; set; } = new List<Rack>();
}