using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardRegisterAsyncServiceBase : IServerCoreStandardRegisterAsyncService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; } = new XFEServerCoreImpl();
    /// <inheritdoc/>
    public abstract Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r);
}
