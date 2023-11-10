using TranqService.Common.Extensions;

namespace TranqService.Shared;
public static class ContainerConfig
{
    public static void ConfigureShared(this ContainerBuilder builder)
    {
        // Bulk registrations
        builder.BulkRegister(
            "TranqService.Shared"
        );
    }
}