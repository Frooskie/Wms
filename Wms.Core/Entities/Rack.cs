namespace Wms.Core.Entities;

public class Rack
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // буквенное обозначение, например "A"
    public int ZoneId { get; set; }

    public Zone Zone { get; set; } = null!;
    public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}