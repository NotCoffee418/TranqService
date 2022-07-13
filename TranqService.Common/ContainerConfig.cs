using TranqService.Common.Extensions;

namespace TranqService.Common;

public static class ContainerConfig
{
    public static void ConfigureCommon(this ContainerBuilder builder)
    {
        // Register runner
        builder.BulkRegister(
            "TranqService.Common.Data"
            );
    }
}
