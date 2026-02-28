using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心校验服务基类
/// </summary>
public abstract class ServerCoreVerifyAsyncServiceBase : XFEServerCoreServiceBase, IServerCoreVerifyAsyncService
{
    /// <inheritdoc/>
    public abstract Task<bool> VerifyRequestAsync(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r);
}
