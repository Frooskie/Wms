using System.Text.Json.Serialization;

namespace Wms.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupplyRequestStatus
{
    Draft,
    Submitted,
    Approved,
    Completed,
    Rejected
}