using System.Text.Json.Serialization;

namespace Wms.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupplyOrderStatus
{
    Draft,
    Confirmed,
    Shipped
}