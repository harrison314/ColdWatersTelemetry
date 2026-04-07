namespace ColdWatersModWebApp.Services;

public class ColdWatersSettings
{
    public string Endpoint
    {
        get;
        set;
    }

    public int MaxMessages
    {
        get;
        set;
    }

    public string[] DisableMessagesColors
    {
        get;
        set;
    }

    public ColdWatersSettings()
    {
        this.Endpoint = string.Empty;
        this.DisableMessagesColors = Array.Empty<string>();
    }
}
