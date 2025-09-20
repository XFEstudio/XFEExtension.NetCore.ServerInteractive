using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerCoreService;

/// <summary>
/// 服务器核心服务基类
/// </summary>
public abstract class ServerCoreServiceBase : IServerCoreService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; init; } = new XFEServerCoreImpl();
    /// <inheritdoc/>
    public abstract void RequestReceived(object? sender, CyberCommRequestEventArgs e);
    /// <inheritdoc/>
    public abstract void ServerStarted(object? sender, EventArgs e);
}
