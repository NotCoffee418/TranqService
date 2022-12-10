// Prepare configuration files
AppPaths sessionAppPaths = await AppPaths.GetAsync();
ApiKeys apiKeys = await ApiKeys.GetAsync();

// Get log file path for this session
string logFilePath = LogFileManager.CleanupAndGetNewLogFilePath();

// init logger
var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Information)
    .WriteTo.File(logFilePath, LogEventLevel.Debug);

// Attempt to add discord logging
if (apiKeys.DiscordWebhookId > 0 && !string.IsNullOrEmpty(apiKeys.DiscordWebhookSecret))
    loggerConfig = loggerConfig
        .WriteTo.Discord(apiKeys.DiscordWebhookId, apiKeys.DiscordWebhookSecret, 
        restrictedToMinimumLevel: LogEventLevel.Warning);
Log.Logger = loggerConfig.CreateLogger();


// Init host
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<YoutubeDownloadService>();
    })
    .UseWindowsService(options =>
    {
        options.ServiceName = "TranqService";
    })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.ConfigureShared();
        builder.ConfigureDatabase();
        builder.ConfigureCommon();
    })
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
