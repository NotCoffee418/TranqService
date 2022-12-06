using TranqService.Common.DataAccess;

namespace TranqService.Tests.Common;

public class PathHelperTests
{
    const string TestingBasePath = "UnitTestData";

    [Fact]
    public void GetAppDataPath_FileInRoot()
    {
        string path = PathHelper.GetAppdataPath(true, TestingBasePath, "nonfile");
        Assert.NotNull(path);
        Assert.True(path.EndsWith(TestingBasePath + "\\nonfile"));
        Assert.True(Directory.GetParent(path).Exists);
    }

    [Fact]
    public void GetAppDataPath_DirectoryInRoot()
    {
        string path = PathHelper.GetAppdataPath(false, TestingBasePath, "subdir");
        Assert.NotNull(path);
        Assert.True(path.EndsWith(TestingBasePath + "\\subdir"));
        Assert.True(Directory.Exists(path));
    }
}
