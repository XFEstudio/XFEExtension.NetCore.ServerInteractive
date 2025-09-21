using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

/// <summary>
/// 异步服务器服务基类
/// </summary>
public abstract class AsyncServerServiceBase : IAsyncServerService
{
    /// <inheritdoc/>
    public XFEServer XFEServer { get; set; }
    /// <inheritdoc/>
    public abstract Task StartServiceAsync();
}
