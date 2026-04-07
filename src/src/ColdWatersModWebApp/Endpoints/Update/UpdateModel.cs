using System.Text.Json.Serialization;

namespace ColdWatersModWebApp.Endpoints.Update;

public class UpdateModel
{
    [JsonPropertyName("submarineIsOnline")]
    public bool SubmarineIsOnline
    {
        get;
        set;
    }

    [JsonPropertyName("curse")]
    public string Curse
    {
        get;
        set;
    }

    [JsonPropertyName("depth")]
    public string Depth
    {
        get;
        set;
    }

    [JsonPropertyName("speed")]

    public string Speed
    {
        get;
        set;
    }

    [JsonPropertyName("rudderAngle")]

    public string RudderAngle
    {
        get;
        set;
    }

    [JsonPropertyName("diveAngle")]
    public string DiveAngle
    {
        get;
        set;
    }

    [JsonPropertyName("breefing")]
    public string Breefing
    {
        get;
        set;
    }

    [JsonPropertyName("orders")]
    public string Orders
    {
        get;
        set;
    }

    [JsonPropertyName("messages")]
    public List<string> Messages
    {
        get;
        set;
    }

    public UpdateModel()
    {
        this.Curse = string.Empty;
        this.Depth = string.Empty;
        this.Speed = string.Empty;
        this.Breefing = string.Empty;
        this.Orders = string.Empty;
        this.DiveAngle = string.Empty;
        this.RudderAngle = string.Empty;
        this.Messages = new List<string>();
    }
}

[JsonSerializable(typeof(UpdateModel))]
internal partial class UpdateModelContext : JsonSerializerContext
{

}