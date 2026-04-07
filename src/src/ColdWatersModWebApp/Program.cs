using ColdWatersModWebApp.Endpoints.Update;
using ColdWatersModWebApp.Worker;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

namespace ColdWatersModWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

        builder.Services
          .AddOptions<Services.ColdWatersSettings>()
          .BindConfiguration("ColdWatersSettings");

        //builder.Services.ConfigureHttpJsonOptions(options =>
        //{
        //    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        //});


        builder.Services.AddHttpClient("SSE").ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(20),
                ConnectTimeout = TimeSpan.FromSeconds(10)
            };
        });

        builder.Services.AddSingleton<EventHub<UpdateModel>>();
        builder.Services.AddHostedService<UpdateWorker>();


        WebApplication app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        Endpoints.Update.Endpoint.Register(app);
        app.Run();
    }
}
