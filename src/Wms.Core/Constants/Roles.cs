namespace Wms.Core.Constants;

/// <summary>
/// Константы ролей пользователей. Используются в [Authorize(Roles = ...)],
/// </summary>
public static class Roles
{
    public const string Chief = "Chief";
    public const string Manager = "Manager";
    public const string Worker = "Worker";
    public const string StoreDirector = "StoreDirector";

    /// <summary>Все допустимые роли.</summary>
    public static readonly IReadOnlyList<string> All = [Chief, Manager, Worker, StoreDirector];
}