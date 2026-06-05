using XFEExtension.NetCore.CyberComm;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器核心服务
/// </summary>
public interface IServerCoreOriginalService : IXFEServerCoreServiceBase
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

    /// <summary>
    /// WebSocket客户端连接事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ClientConnected(object? sender, CyberCommServerEventArgs e) { }

    /// <summary>
    /// WebSocket消息接收事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void MessageReceived(object? sender, CyberCommServerEventArgs e) { }

    /// <summary>
    /// WebSocket连接关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ConnectionClosed(object? sender, CyberCommServerEventArgs e) { }
}
