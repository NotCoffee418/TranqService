// Get appsettings path from commandline args
int configPathKeyIndex = Array.IndexOf(args, "--configPath");
if (configPathKeyIndex == -1 || configPathKeyIndex + 1 < args.Length - 1) 
    throw new Exception("No config path specified.");
string configPath = args[configPathKeyIndex + 1];

// Access config manually for DI building
IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile(configPath)
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
    .UseWindowsService(options =>
    {
        options.ServiceName = "TranqService";
    })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
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
