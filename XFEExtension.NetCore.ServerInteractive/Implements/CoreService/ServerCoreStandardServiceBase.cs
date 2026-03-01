using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardServiceBase : XFEServerCoreServiceBase, IServerCoreStandardService
{
    /// <inheritdoc/>
    public void Initialize() { }
    /// <inheritdoc/>
    public virtual void RequestReceive() { }
    /// <inheritdoc/>
    public virtual async Task RequestReceiveAsync() { }
}