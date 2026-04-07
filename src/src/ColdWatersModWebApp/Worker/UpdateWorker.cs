using ColdWatersModWebApp.Endpoints.Update;
using ColdWatersModWebApp.Services;
using Microsoft.Extensions.Options;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading.Channels;

namespace ColdWatersModWebApp.Worker;

public class UpdateWorker : BackgroundService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly EventHub<UpdateModel> writer;
    private readonly IOptions<ColdWatersSettings> settings;
    private readonly ILogger<UpdateWorker> logger;

    public UpdateWorker(IHttpClientFactory httpClientFactory,
        EventHub<UpdateModel> writer,
        IOptions<ColdWatersSettings> settings,
        ILogger<UpdateWorker> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.writer = writer;
        this.settings = settings;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        UpdateModel model = new UpdateModel()
        {
            SubmarineIsOnline = false,
            Curse = "-",
            Depth = "-",
            DiveAngle = "-",
            RudderAngle = "-",
            Speed = "_",
            Breefing = "...",
            Orders = "..."
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                HttpClient httpClient = this.httpClientFactory.CreateClient("SSE");

                using Stream stream = await httpClient.GetStreamAsync(this.settings.Value.Endpoint, stoppingToken);
                this.logger.LogTrace("Open SSE on {Endpoint}", this.settings.Value.Endpoint);

                this.NotifyModel(model, m => m.SubmarineIsOnline = true);

                await foreach (SseItem<string> item in SseParser.Create(stream).EnumerateAsync(stoppingToken))
                {

                    this.logger.LogInformation("Message from submarine: {0} {1}", item.EventType, item.Data);
                    try
                    {
                        ColdWatersTelemetryMessage? message = JsonSerializer.Deserialize(item.Data,
                            ColdWatersTelemetryMessageContext.Default.ColdWatersTelemetryMessage);
                        if (message == null)
                        {
                            this.logger.LogTrace("JsonSerializer.Deserialize returns null");
                            continue;
                        }

                        this.ProcessEvent(ref model, message);
                        this.writer.Notify(model);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Error during processing message: {Message}", item.Data);
                    }
                }
            }
            catch (System.Net.Http.HttpRequestException ex) when (ex.Message.Contains("refused it"))
            {
                this.NotifyModel(model, m => m.SubmarineIsOnline = false);
                this.logger.LogWarning("Can not connect {0} Error: {1}",
                    this.settings.Value.Endpoint,
                    ex.Message);
                await Task.Delay(2000, stoppingToken);
            }
            catch (Exception ex)
            {
                this.NotifyModel(model, m => m.SubmarineIsOnline = false);
                this.logger.LogError(ex, "Error during connections");
                await Task.Delay(2000, stoppingToken);
            }
            finally
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(2000, stoppingToken);
                    this.logger.LogTrace("reconecting");
                }
            }
        }
    }

    private void ProcessEvent(ref UpdateModel model, ColdWatersTelemetryMessage message)
    {
        if (message is SpeedChangedMessage speedChangedMessage)
        {
            model.Speed = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0.0}",
                    Math.Round(speedChangedMessage.Value, 1));
        }

        if (message is DepthChangedMessage depthChangedMessage)
        {
            model.Depth = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0}",
                    Math.Round(depthChangedMessage.Value));
        }

        if (message is RudderChangedMessage rudderChangedMessage)
        {
            model.DiveAngle = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0}°",
                    Math.Round(rudderChangedMessage.DiveAngle));
            model.RudderAngle = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0}°",
                    Math.Round(rudderChangedMessage.RudderAngle));
        }

        if (message is CurseChangeMessage curseChangeMessage)
        {
            model.Curse = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                   "{0:0}",
                   Math.Round(curseChangeMessage.Value));
        }

        if (message is BreefingChangeMessage breefingChangeMessage)
        {
            model.Breefing = breefingChangeMessage.Value ?? string.Empty;
        }

        if (message is MessageChangeMessage messageChangeMessage && messageChangeMessage.Value != null)
        {
            if (!this.settings.Value.DisableMessagesColors.Contains(messageChangeMessage.Color, StringComparer.OrdinalIgnoreCase)) // Skip info messages
            {
                model.Messages.Add(messageChangeMessage.Value);
                while (model.Messages.Count > this.settings.Value.MaxMessages)
                {
                    model.Messages.RemoveAt(0);
                }
            }
        }

        if (message is ToBattlestationsMessage _)
        {
            model.Messages.Clear();
        }

        if (message is MissionOrdersChangeMessage missionOrdersChangeMessage)
        {
            model.Orders = missionOrdersChangeMessage.Value ?? string.Empty;
        }
    }

    private void NotifyModel(UpdateModel model, Action<UpdateModel> setup)
    {
        setup(model);
        this.writer.Notify(model);
    }
}
