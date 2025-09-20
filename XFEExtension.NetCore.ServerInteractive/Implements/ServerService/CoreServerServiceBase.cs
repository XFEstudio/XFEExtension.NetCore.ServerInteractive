using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 核心服务器服务基类
/// </summary>
public abstract class CoreServerServiceBase : ICoreServerService
{
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; } = new XFEServerImpl();
    /// <inheritdoc/>
    public string CoreServerName { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string BindingIPAddress { get; set; } = string.Empty;

    /// <inheritdoc/>
    public abstract Task StartServerCore();
}
