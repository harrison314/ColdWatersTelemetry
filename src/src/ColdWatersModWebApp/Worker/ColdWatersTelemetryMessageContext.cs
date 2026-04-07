using System.Text.Json.Serialization;

namespace ColdWatersModWebApp.Worker;

[JsonSerializable(typeof(ColdWatersTelemetryMessage))]
internal partial class ColdWatersTelemetryMessageContext : JsonSerializerContext
{

}