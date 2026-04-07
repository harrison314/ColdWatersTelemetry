using ColdWatersModWebApp.Worker;
using Microsoft.AspNetCore.Mvc;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading.Channels;

namespace ColdWatersModWebApp.Endpoints.Update;

internal static class Endpoint
{
    internal static void Register(WebApplication app)
    {
        async IAsyncEnumerable<string> StreamEvents(EventHub<UpdateModel> eventHub)
        {
            await foreach(UpdateModel model in eventHub.Subscribe())
            {
                string json = JsonSerializer.Serialize(model, UpdateModelContext.Default.UpdateModel);
                yield return json;
            }
        }

        app.MapGet("/events", ([FromServices] EventHub<UpdateModel> eventHub) =>
        {
            return TypedResults.ServerSentEvents<string>(StreamEvents(eventHub), null);
        });
    }
}
