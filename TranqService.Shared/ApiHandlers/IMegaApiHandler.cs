using CG.Web.MegaApiClient;

namespace TranqService.Shared.ApiHandlers
{
    public interface IMegaApiHandler
    {
        Task<MegaApiClient.LogonSessionToken> AuthenticateAsync();
        Task<INode> FindDirectoryNode(string dirPath, List<INode> allNodes = null);
        Task<List<INode>> GetAllNodes();
        Task<List<INode>> ListFileNodes(INode parentDirectory, List<INode>? allNodes = null);
        Task<INode> UploadFile(string localPath, INode targetDirectoryNode);
    }
}