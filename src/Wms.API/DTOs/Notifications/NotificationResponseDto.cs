namespace Wms.API.DTOs.Notifications;

public record NotificationResponseDto(
    int Id,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);