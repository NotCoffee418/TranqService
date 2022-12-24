using TranqService.Shared.DataAccess.ApiHandlers;

namespace TranqService.Tests.Shared.DataAccess;

public class GithubAccessTests
{

    [Fact(Skip = "This doesn't run from github with http error 'forbidden'")]
    public async Task GetLatestYtDlpVersionAsync_ExpectAcceptableValue()
    {
        // Create instance
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        var github = mock.Create<IGithubAccess>();

        DateTime latestRelease = await github.GetLatestYtDlpVersionAsync();
        Assert.NotNull(latestRelease);
        Assert.True(latestRelease > DateTime.Parse("2022-11-01"));
    }
}