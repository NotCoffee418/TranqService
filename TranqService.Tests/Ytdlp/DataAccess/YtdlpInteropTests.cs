using TranqService.Shared.DataAccess.Ytdlp;
using TranqService.Shared.Logic;

namespace TranqService.Tests.Ytdlp.DataAccess;

public class YtdlpInteropTests
{
    const string TestVideoUrl = "https://www.youtube.com/watch?v=C0DPdy98e4c";
    const string TestBrokenUrl = "https://www.youtube.com/watch?v=NotARealId";


    [Fact]
    public async Task DownloadVideo_WorkingVideo_ManuallyValidate()
    {
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        var updater = mock.Create<IYtdlpUpdater>();
        var pathHelper = mock.Create<IPathHelper>();
        var ytdlpInterop = mock.Create<IYtdlpInterop>();

        // Ensure installation is complete
        await updater.TryUpdateYtdlpAsync();

        string videoSavePath = pathHelper.GetAppdataPath(true, "UnitTestDownloads", "test-working-video.mp4");
        string audioSavePath = pathHelper.GetAppdataPath(true, "UnitTestDownloads", "test-working-audio.mp3");

        // Download the content
        await ytdlpInterop.DownloadVideoAsync(TestVideoUrl, videoSavePath);
        await ytdlpInterop.DownloadAudioAsync(TestVideoUrl, audioSavePath);

        Assert.True(File.Exists(videoSavePath));
        Assert.True(File.Exists(audioSavePath));
    }

}
