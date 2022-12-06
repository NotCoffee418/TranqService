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
        var ytdlInterop = mock.Create<IYtdlpInterop>();

        // Run updater
        await updater.TryUpdateYtdlpAsync();

        // Assertion prep
        string versionFilePath = ytdlpPaths.GetYtdlpVersionFilePath();
        string exePath = ytdlpPaths.GetYtdlpExePath();

        // Validate returned paths
        Assert.NotNull(versionFilePath);
        Assert.NotNull(exePath);

        // Ensure version file exists and is parsable to date
        Assert.True(File.Exists(versionFilePath));
        Assert.True(DateTime.TryParse(await File.ReadAllTextAsync(versionFilePath), out _));

        // Ensure yt-dlp exe
        Assert.True(File.Exists(exePath));
        Assert.True(new FileInfo(exePath).Length > 1);

        // Ensure ffmpeg is installed
        Assert.True(await ytdlInterop.ValidateFfmpegInstallationAsync());
    }
}
