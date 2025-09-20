using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardRegisterServiceBase : IServerCoreStandardRegisterService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; } = new XFEServerCoreImpl();
    /// <inheritdoc/>
    public abstract void StandardRequestReceived(object? sender, string execute, QueryableJsonNode queryableJsonNode, CyberCommRequestEventArgs e);
}
