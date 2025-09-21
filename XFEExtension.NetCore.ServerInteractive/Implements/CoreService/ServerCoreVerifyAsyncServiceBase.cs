using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心校验服务基类
/// </summary>
public abstract class ServerCoreVerifyAsyncServiceBase : IServerCoreVerifyAsyncService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; } = new XFEServerCoreImpl();
    /// <inheritdoc/>
    public abstract Task<bool> VerifyRequestAsync(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r);
}
