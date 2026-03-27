using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心校验服务基类
/// </summary>
public abstract class ServerCoreVerifyServiceBase : XFEServerCoreServiceBase, IServerCoreVerifyService
{
    /// <inheritdoc/>
    public virtual bool VerifyRequest() => true;

    /// <inheritdoc/>
    public virtual Task<bool> VerifyRequestAsync() => Task.FromResult(true);
}
