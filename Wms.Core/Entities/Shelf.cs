namespace Wms.Core.Entities;

public class Shelf
{
    public int Id { get; set; }
    public int Number { get; set; } // номер полки на стеллаже
    public int RackId { get; set; }
    
    public Rack Rack { get; set; } = null!;
    public ICollection<Cell> Cells { get; set; } = new List<Cell>();
}