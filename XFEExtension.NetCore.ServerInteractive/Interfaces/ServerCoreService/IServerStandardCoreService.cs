using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;

/// <summary>
/// 服务器标准核心服务
/// </summary>
public interface IServerStandardCoreService : IXFEServerCoreService
{
    /// <summary>
    /// 服务器标准请求接收事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="execute"></param>
    /// <param name="queryableJsonNode"></param>
    /// <param name="e"></param>
    void StandardRequestReceived(object? sender, string execute, QueryableJsonNode? queryableJsonNode, CyberCommRequestEventArgs e);
}
