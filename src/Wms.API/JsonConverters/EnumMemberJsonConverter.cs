using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wms.API.JsonConverters;

/// <summary>
/// JSON-конвертер для enum'ов, учитывающий значения из [EnumMember(Value = "...")].
/// На .NET 8 стандартный JsonStringEnumConverter игнорирует [EnumMember],
/// поэтому нужен собственный конвертер.
/// </summary>
public class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, string> ToStringMap = new();
    private static readonly Dictionary<string, TEnum> FromStringMap = new(StringComparer.OrdinalIgnoreCase);

    static EnumMemberJsonConverter()
    {
        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = (TEnum)field.GetValue(null)!;
            var name = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name;

            ToStringMap[value] = name;
            FromStringMap[name] = value;
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str is null || !FromStringMap.TryGetValue(str, out var value))
            throw new JsonException($"Недопустимое значение {typeof(TEnum).Name}: '{str}'.");
        return value;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (ToStringMap.TryGetValue(value, out var str))
            writer.WriteStringValue(str);
        else
            writer.WriteStringValue(value.ToString());
    }
}