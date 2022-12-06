namespace TranqService.Ytdlp;
public static class ContainerConfig
{
    public static void ConfigureYtdlp(this ContainerBuilder builder)
    {
        // Bulk registrations
        builder.BulkRegister(
            "TranqService.Ytdlp"
        );
    }
}