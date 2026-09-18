using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Wms.API.JsonConverters;
using Wms.Core.Constants;

namespace Wms.API.DTOs.Common;

/// <summary>
/// Расширенный ProblemDetails (RFC 7807) с полями, специфичными для WMS API.
/// Используется как единый формат ответа об ошибке для всех исключений.
/// </summary>
/// <remarks>
/// Наследуется от <see cref="ProblemDetails"/>, чтобы сохранить совместимость
/// со стандартными полями (<c>type</c>, <c>title</c>, <c>status</c>, <c>instance</c>, <c>detail</c>).
/// </remarks>
public class WmsProblemDetails : ProblemDetails
{
    /// <summary>
    /// Машиночитаемый код ошибки. Используется клиентом (фронтендом) для
    /// выбора UX-сценария независимо от текста сообщения.
    /// </summary>
    /// <example>NOT_FOUND</example>
    [JsonConverter(typeof(EnumMemberJsonConverter<ErrorCodes>))]
    public ErrorCodes? Code { get; set; }

    /// <summary>
    /// Идентификатор запроса для корреляции с логами сервера.
    /// </summary>
    /// <example>0HLO1K7ABCDEF:00000001</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }

    /// <summary>
    /// Ошибки валидации по полям. Заполняется только для ошибок типа
    /// <see cref="Core.Exceptions.ModelValidationException"/>.
    /// Ключ — имя поля, значение — массив сообщений.
    /// </summary>
    /// <example>{ "Name": ["'Название' не должно быть пустым."] }</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }
}