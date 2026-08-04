namespace Wms.Core.Entities;

public class Cell
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // полный адрес ячейки (стеллаж_поляка_ячейка)
    public int ShelfId { get; set; }
    public bool IsOccupied { get; set; }

    public Shelf Shelf { get; set; } = null!;
}