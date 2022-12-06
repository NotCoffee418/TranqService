namespace TranqService.Tests.Ytdlp.Logic;

public class YtdlpUpdaterTests
{
    [Fact]
    public async Task TryUpdateYtdlpAsync_SimpleInstallCheck()
    {
        // Create instance
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        var updater = mock.Create<IYtdlpUpdater>();
        var ytdlpPaths = mock.Create<IYtdlpPaths>();

        // Run updater
        await updater.TryUpdateYtdlpAsync();

        // Assertion prep
        string versionFilePath = ytdlpPaths.GetYtdlpVersionFilePath();
        string exePath = ytdlpPaths.GetYtdlpExePath();

        Assert.NotNull(versionFilePath);
        Assert.NotNull(exePath);
        Assert.True(File.Exists(versionFilePath));
        Assert.True(File.Exists(exePath));
        Assert.True(new FileInfo(exePath).Length > 1);
    }
}
