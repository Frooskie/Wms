namespace Wms.Core.Entities;

public record Notification
{
    public int Id { get; init; }
    public string UserId { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Message { get; init; } = null!;
    public bool IsRead { get; set; }          // может быть изменено пользователем
    public DateTime CreatedAt { get; init; }
    
    public virtual ApplicationUser? User { get; init; }
}