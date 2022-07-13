namespace TranqService.Database;

public class Db : IDb, IDesignTimeDbContextFactory<TranqDbContext>
{
    private IConfiguration _configuration;

    /// <summary>
    /// Empty constructor required by Microsoft.EntityFrameworkCore.Design
    /// </summary>
    public Db()
    {
        string appsettingsPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appsettings.json");
        IConfigurationBuilder? localConfBuilder = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: false);
        _configuration = localConfBuilder.Build();
    }

    /// <summary>
    /// Normal constructor initialized by autofac
    /// </summary>
    /// <param name="configuration"></param>
    public Db(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static bool FirstRequest { get; set; } = true;
    private static bool UpgradingDatabase { get; set; } = false;

    static Mutex mutex = new Mutex();


    /// <summary>
    /// Used by application
    /// </summary>
    /// <returns></returns>
    public TranqDbContext GetContext()
    {
        mutex.WaitOne();
        var context = new TranqDbContext(_configuration);

        // Apply migrations on first request
        if (FirstRequest)
        {
            UpgradingDatabase = true;
            FirstRequest = false;
            _ = Task.Run(() =>
            {
                context.Database.Migrate();
                UpgradingDatabase = false;
            });
        }
        while (UpgradingDatabase)
            Task.Delay(50).Wait();

        mutex.ReleaseMutex();
        return context;
    }


    /// <summary>
    /// Used by IDesignTimeDbContextFactory to create migrations. Should not be used in code.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public TranqDbContext CreateDbContext(string[] args)
        => new TranqDbContext(_configuration);
}
