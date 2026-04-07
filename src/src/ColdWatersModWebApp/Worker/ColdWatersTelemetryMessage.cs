using System.Text.Json.Serialization;

namespace ColdWatersModWebApp.Worker;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kid")]
[JsonDerivedType(typeof(DepthUnderKeelWarnedMessage), typeDiscriminator: "depthUnderKeelWarned")]
[JsonDerivedType(typeof(IceWarnedMessage), typeDiscriminator: "iceWarned")]
[JsonDerivedType(typeof(MineWarnedMessage), typeDiscriminator: "mineWarned")]
[JsonDerivedType(typeof(SpeedChangedMessage), typeDiscriminator: "speed")]
[JsonDerivedType(typeof(DepthChangedMessage), typeDiscriminator: "depth")]
[JsonDerivedType(typeof(RudderChangedMessage), typeDiscriminator: "rudder")]
[JsonDerivedType(typeof(BallastChangedMessage), typeDiscriminator: "ballast")]
[JsonDerivedType(typeof(SonarPingMessage), typeDiscriminator: "sonarPing")]
[JsonDerivedType(typeof(CurseChangeMessage), typeDiscriminator: "curse")]
[JsonDerivedType(typeof(BreefingChangeMessage), typeDiscriminator: "breefing")]
[JsonDerivedType(typeof(MessageChangeMessage), typeDiscriminator: "message")]
[JsonDerivedType(typeof(ToBattlestationsMessage), typeDiscriminator: "toBattlestations")]
[JsonDerivedType(typeof(MissionOrdersChangeMessage), typeDiscriminator: "missionOrders")]
public abstract partial class ColdWatersTelemetryMessage
{
}

public partial class DepthUnderKeelWarnedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public bool Value
    {
        get;
        set;
    }
}

public partial class IceWarnedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public bool Value
    {
        get;
        set;
    }
}

public partial class MineWarnedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public bool Value
    {
        get;
        set;
    }
}

public partial class SpeedChangedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public double Value
    {
        get;
        set;
    }
}

public partial class DepthChangedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public double Value
    {
        get;
        set;
    }
}

public partial class RudderChangedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("rudderAngle")]
    public double RudderAngle
    {
        get;
        set;
    }

    [JsonPropertyName("diveAngle")]
    public double DiveAngle
    {
        get;
        set;
    }
}

public partial class BallastChangedMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("angle")]
    public double? Angle
    {
        get;
        set;
    }

    [JsonPropertyName("time")]
    public double? Time
    {
        get;
        set;
    }
}

public partial class SonarPingMessage : ColdWatersTelemetryMessage
{

}

public partial class CurseChangeMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public double Value
    {
        get;
        set;
    }
}

public partial class BreefingChangeMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public string? Value
    {
        get;
        set;
    }
}

public partial class MessageChangeMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public string? Value
    {
        get;
        set;
    }

    [JsonPropertyName("color")]
    public string? Color
    {
        get;
        set;
    }
}

public partial class ToBattlestationsMessage : ColdWatersTelemetryMessage
{

}

public partial class MissionOrdersChangeMessage : ColdWatersTelemetryMessage
{
    [JsonPropertyName("value")]
    public string? Value
    {
        get;
        set;
    }
}
