using TranqService.Common.DataAccess;
using TranqService.Shared.DataAccess.Ytdlp;
using TranqService.Shared.Logic;

namespace TranqService.Tests.Shared.DataAccess;

public class YtdlpInteropTests
{
    const string TestVideoUrl = "https://www.youtube.com/watch?v=C0DPdy98e4c";
    const string TestBrokenUrl = "https://www.youtube.com/watch?v=NotARealId";


    [Fact]
    public async Task DownloadVideo_WorkingVideo_ManuallyValidate()
    {
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        var updater = mock.Create<IYtdlpUpdater>();
        var ytdlpInterop = mock.Create<IYtdlpInterop>();

        // Ensure installation is complete
        await updater.TryUpdateYtdlpAsync();

        string videoSavePath = PathHelper.GetAppdataPath(true, "UnitTestDownloads", "test-working-video.mp4");
        string audioSavePath = PathHelper.GetAppdataPath(true, "UnitTestDownloads", "test-working-audio.mp3");

        // Download the content
        (bool audioSuccess, string audioError) = await ytdlpInterop.DownloadAudioAsync(TestVideoUrl, audioSavePath);
        Assert.True(audioSuccess);

        (bool videoSuccess, string videoError) = await ytdlpInterop.DownloadVideoAsync(TestVideoUrl, videoSavePath);
        Assert.True(videoSuccess);

        Assert.True(File.Exists(videoSavePath));
        Assert.True(File.Exists(audioSavePath));
    }

}
