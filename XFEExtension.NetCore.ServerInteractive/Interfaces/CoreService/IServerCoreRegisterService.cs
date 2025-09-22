using XFEExtension.NetCore.CyberComm;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器核心服务
/// </summary>
public interface IServerCoreRegisterService : IXFEServerCoreServiceBase
{
    /// <summary>
    /// 服务器启动事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ServerStarted(object? sender, EventArgs e);

    /// <summary>
    /// 服务器接收到请求事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void RequestReceived(object? sender, CyberCommRequestEventArgs e);
}
