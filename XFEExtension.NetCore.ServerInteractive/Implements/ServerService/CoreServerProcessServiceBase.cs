using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 核心服务器处理服务基类
/// </summary>
[CreateImpl]
public abstract class ServerCoreProcessServiceBase : IServerCoreProcessService
{
    /// <inheritdoc/>
    public IReadOnlyList<IServerCoreService> ServerCoreServiceList { get; set; } = [];
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; }
    /// <inheritdoc/>
    public abstract Task ProcessServerCore();
}
