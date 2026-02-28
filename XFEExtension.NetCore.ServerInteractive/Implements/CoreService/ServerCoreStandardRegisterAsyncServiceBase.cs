using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardRegisterAsyncServiceBase : XFEServerCoreServiceBase, IServerCoreStandardRegisterAsyncService
{
    /// <inheritdoc/>
    public abstract Task StandardRequestReceived();
}
