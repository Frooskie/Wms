using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wms.API.Swagger;

/// <summary>
/// Учит Swashbuckle использовать значения из [EnumMember(Value = "...")]
/// вместо C#-имён членов enum. Приводит схему Swagger в соответствие
/// с фактической JSON-сериализацией (JsonStringEnumConverter).
/// </summary>
public class EnumMemberSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;
        if (!type.IsEnum) return;

        // Ищем [EnumMember] на каждом члене enum
        var enumValues = type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => new
            {
                Name = f.Name,
                Value = f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name
            })
            .ToList();

        if (enumValues.Count == 0) return;

        // Значения как строки
        schema.Enum = enumValues
            .Select(v => (IOpenApiAny)new OpenApiString(v.Value))
            .ToList();

        // Подсказки в UI: "NOT_FOUND (NotFound)"
        schema.Description = string.Join(", ", enumValues.Select(v => v.Value));

        // Схема — строка, не число
        schema.Type = "string";
        schema.Format = null;
    }
}