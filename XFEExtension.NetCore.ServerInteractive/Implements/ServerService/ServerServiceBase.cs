using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 服务器服务基类
/// </summary>
public abstract class ServerServiceBase : IServerService
{
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; }
    /// <inheritdoc/>
    public abstract void StartService();
}
