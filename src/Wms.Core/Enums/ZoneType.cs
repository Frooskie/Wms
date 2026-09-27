using System.Text.Json.Serialization;

namespace Wms.Core.Enums;

/// <summary>Тип зоны хранения.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ZoneType
{
    /// <summary>Обычная зона.</summary>
    Normal,

    /// <summary>Холодильник.</summary>
    Fridge,

    /// <summary>Морозилка.</summary>
    Freezer
}