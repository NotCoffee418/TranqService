namespace TranqService.Tests.Shared.DataAccess;

public class PathHelperTests
{
    private IPathHelper _pathHelper;

    public PathHelperTests()
    {
        // Create instance
        using var mock = AutoMock.GetLoose(builder => builder.AddMocked());
        _pathHelper = mock.Create<IPathHelper>();
    }

    const string TestingBasePath = "UnitTestData";

    [Fact]
    public void GetAppDataPath_FileInRoot()
    {
        string path = _pathHelper.GetAppdataPath(true, TestingBasePath, "nonfile");
        Assert.NotNull(path);
        Assert.True(path.EndsWith(TestingBasePath + "\\nonfile"));
        Assert.True(Directory.GetParent(path).Exists);
    }

    [Fact]
    public void GetAppDataPath_DirectoryInRoot()
    {
        string path = _pathHelper.GetAppdataPath(false, TestingBasePath, "subdir");
        Assert.NotNull(path);
        Assert.True(path.EndsWith(TestingBasePath + "\\subdir"));
        Assert.True(Directory.Exists(path));
    }
}
