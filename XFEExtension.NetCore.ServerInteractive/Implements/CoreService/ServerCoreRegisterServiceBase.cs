using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心服务基类
/// </summary>
public abstract class ServerCoreRegisterServiceBase : IServerCoreRegisterService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; }
    /// <inheritdoc/>
    public abstract void ServerStarted(object? sender, EventArgs e);
    /// <inheritdoc/>
    public abstract void RequestReceived(object? sender, CyberCommRequestEventArgs e);
}
