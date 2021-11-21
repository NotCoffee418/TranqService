using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using TranqService.Shared;


// Init host
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<YoutubeDownloadService>();
    })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(ContainerConfig.Configure)
    .UseSerilog(Log.Logger)
    .Build();

// Load config
IConfig config = (IConfig)host.Services.GetRequiredService(typeof(IConfig));

// init logger
Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Discord(config.DiscordWebhookId, config.DiscordWebhookSecret, restrictedToMinimumLevel: LogEventLevel.Warning)
        .CreateLogger();

// Notify of restart
Log.Logger.Warning("Tranqservice (re)started.");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Logger.Fatal(
        "TranqService has crashed!" + Environment.NewLine + "{0}" + Environment.NewLine + "{1}", 
        ex.Message, ex.StackTrace);
}
