using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using System.Reflection;
using TranqService.Shared;

// Access config manually for DI building
string appsettingsPath = "/app/appsettings.json";
IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile(appsettingsPath)
        .Build();
ulong discordWebhookId = Convert.ToUInt64(configuration["Config:DiscordWebhookId"]);
string discordWebhookSecret = configuration["Config:DiscordWebhookSecret"];

// init logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Information)
    .WriteTo.Discord(discordWebhookId, discordWebhookSecret, 
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();

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
