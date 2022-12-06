using System.IO;
using TranqService.Ytdlp.Logic;

namespace TranqService.Tests.Ytdlp.Logic;

public class YtdlpUpdaterTests
{
    [Fact]
    public async Task TryUpdateYtdlpAsync_SimpleInstallCheck()
    {
        // Create instance
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        var updater = mock.Create<IYtdlpUpdater>();

        // Run updater
        await updater.TryUpdateYtdlpAsync();

        // Assertion prep
        string versionFilePath = updater.GetYtdlpVersionFilePath();
        string exePath = updater.GetYtdlpExePath();

        Assert.NotNull(versionFilePath);
        Assert.NotNull(exePath);
        Assert.True(File.Exists(versionFilePath));
        Assert.True(File.Exists(exePath));
        Assert.True(new FileInfo(exePath).Length > 1);
    }
}
