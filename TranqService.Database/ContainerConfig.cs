using TranqService.Common.Extensions;

namespace TranqService.Database;

public static class ContainerConfig
{
    public static void ConfigureDatabase(this ContainerBuilder builder)
    {
        // Register runner
        builder.RegisterType<Db>().As<IDb>();
        builder.BulkRegister(
            "TranqService.Database.Queries"
            );
    }
}
