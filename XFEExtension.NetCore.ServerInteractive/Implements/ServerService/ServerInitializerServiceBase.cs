using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 服务器初始化服务基类
/// </summary>
public abstract class ServerInitializerServiceBase : IServerInitializerService
{
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; }
    /// <inheritdoc/>
    public abstract void Initialize();
}
