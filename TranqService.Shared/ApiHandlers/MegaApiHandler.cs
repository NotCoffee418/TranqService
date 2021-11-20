using CG.Web.MegaApiClient;

namespace TranqService.Shared.ApiHandlers;
public class MegaApiHandler : IMegaApiHandler
{
    private ILogger _logger;
    private IConfig _config;
    private MegaApiClient client = new MegaApiClient();

    public MegaApiHandler(
        ILogger logger,
        IConfig config)
    {
        _logger = logger;
        _config = config;

        // Authenticate
        AuthenticateAsync().Wait();
    }

    public async Task<MegaApiClient.LogonSessionToken> AuthenticateAsync()
    {
        try
        {
            var authInfos = await client.GenerateAuthInfosAsync(_config.MegaUsername, _config.MegaPassword);
            return await client.LoginAsync(authInfos);
        }
        catch (Exception ex)
        {
            _logger.Error("MegaApi: Authentication failed. {0}", ex);
            throw;
        }
    }

    /// <summary>
    /// Navigates to a folder. Create missing parts.
    /// WARNING: This is a very slow operation.
    /// </summary>
    /// <param name="dirPath">/case/insensitive/path/to/directory</param>
    /// <param name="allNodes">Optionally pass all nodes</param>
    /// <returns>node for the selected directory</returns>
    public async Task<INode> FindDirectoryNode(string dirPath, List<INode> allNodes = null)
    {
        // Load all nodes
        if (allNodes == null)
            allNodes = await GetAllNodes();

        // List all directories (slow operation)
        List<INode> allDirectories = allNodes
            .Where(x => x.Type == NodeType.Directory)
            .ToList();

        // Get root node
        INode rootNode = allNodes
            .Where(x => x.Type == NodeType.Root)
            .First();

        // Split inputpath into parts, ignore whitespace
        var requestDirParts = dirPath
            .Replace("\\", "/")
            .Split('/')
            .Where(x => x != string.Empty)
            .ToList();

        // Navigate to the correct dir or create directory
        INode nodeCursor = rootNode;
        for (int i = 0; i < requestDirParts.Count; i++)
        {
            // Navigate to approperiate child node
            INode? finderNode = allDirectories
                .Where(x => x.ParentId == nodeCursor.Id) // In parent folder
                .Where(x => x.Name.ToLower() == requestDirParts[i].ToLower())
                .FirstOrDefault();

            // Directory does not exist, create it
            if (finderNode == null)
            {
                // Warning in case something goes horribly wrong
                _logger.Information($"Created missing folder '{requestDirParts[i]}' in Mega.");
                finderNode = await client.CreateFolderAsync(requestDirParts[i], nodeCursor);
            }

            // Update node cursor
            nodeCursor = finderNode;
        }

        // Return final node cursors
        return nodeCursor;
    }

    /// <summary>
    /// Lists all files in a given directory node
    /// </summary>
    /// <param name="parentDirectory"></param>
    /// <returns></returns>
    public async Task<List<INode>> ListFileNodes(INode parentDirectory, List<INode>? allNodes = null)
    {
        // Validation
        if (parentDirectory.Type == NodeType.File)
            throw new ArgumentException("MegaApiHandler: Provided node type for targetDirectoryNode was not directory.");

        // Load all nodes
        if (allNodes == null)
            allNodes = await GetAllNodes();

        // Filter to only files inside parent dir
        return allNodes
            .Where(x => x.ParentId == parentDirectory.Id)
            .Where(x => x.Type == NodeType.File)
            .ToList();
    }

    /// <summary>
    /// Lists all nodes in mega
    /// </summary>
    /// <returns></returns>
    public async Task<List<INode>> GetAllNodes()
        => (await client.GetNodesAsync()).ToList();

    /// <summary>
    /// Uploads a file to mega
    /// </summary>
    /// <param name="localPath"></param>
    /// <param name="targetFileName"></param>
    /// <param name="targetDirectoryNode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task UploadFile(string localPath, INode targetDirectoryNode)
    {
        // Validation
        if (targetDirectoryNode.Type == NodeType.File)
            throw new ArgumentException("MegaApiHandler: Provided node type for targetDirectoryNode was not directory.");

        await client.UploadFileAsync(localPath, targetDirectoryNode);
    }
}