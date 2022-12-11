// -- Hide the console window unless debugging
// It won't respond correctly to visual studio's debugger regardless
// Reason for this approach:
// We need the app to run as a user. The only way to do that as a windows service is to prompt user for password.
// Having the service run as system results in appdata being stored in system32 somewhere. Do not want that.
// Additionally, it would only allow one install of the service shared between all users.
// Biggest downside is, console window will briefly pop up when the service starts.
// I can think of other approaches but they border on madness for something that should be a lot simpler.
// eg. winforms without a form, but that might break on linux without UI.
// or setting project output type to windows, hides it by default, but also for the debugger (+linux?).
if (!Debugger.IsAttached)
    WindowManager.HideConsole();

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
