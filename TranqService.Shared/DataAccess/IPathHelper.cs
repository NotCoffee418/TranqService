namespace TranqService.Shared.DataAccess
{
    public interface IPathHelper
    {
        string GetAppdataPath(bool isPathFile, params string[]? subPaths);
    }
}