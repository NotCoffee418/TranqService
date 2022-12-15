namespace TranqService.Common.Models.Configs;

[ConfigFile("AppVersionInfo.json")]
public class AppVersionInfo : ConfigBase<AppVersionInfo>
{
    public DateTime? InstalledServiceVersionTime { get; set; } = null;
    public DateTime? InstalledUiVersionTime { get; set; } = null;
}
