using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 核心服务器服务基类
/// </summary>
public abstract class ServerCoreServiceBase : IServerCoreService
{
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; }
    /// <inheritdoc/>
    public string ServerCoreName { get; set; } = string.Empty;
    /// <inheritdoc/>
    public List<string> BindingIPAddressList { get; set; } = [];

    /// <inheritdoc/>
    public abstract Task StartServerCore();
}
