using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 核心服务器处理服务基类
/// </summary>
[CreateImpl]
public abstract class CoreServerProcessServiceBase : ICoreServerProcessService
{
    /// <inheritdoc/>
    public IReadOnlyList<ICoreServerService> CoreServerServiceList { get; set; } = [];
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; } = new XFEServerImpl();
    /// <inheritdoc/>
    public abstract Task ProcessCoreServer();
}
