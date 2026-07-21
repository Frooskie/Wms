namespace Wms.Core.Entities;

public class Cell
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // можно хранить полный адрес или просто номер
    public int ShelfId { get; set; }
    public bool IsOccupied { get; set; }

    public Shelf Shelf { get; set; } = null!;
}