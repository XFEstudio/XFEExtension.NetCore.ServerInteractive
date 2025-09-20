using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerCoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerStandardCoreServiceBase : IServerStandardCoreService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; init; } = new XFEServerCoreImpl();
    /// <inheritdoc/>
    public abstract void StandardRequestReceived(object? sender, string execute, QueryableJsonNode? queryableJsonNode, CyberCommRequestEventArgs e);
}
