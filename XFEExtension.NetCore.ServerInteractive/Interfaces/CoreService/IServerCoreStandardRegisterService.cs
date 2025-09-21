using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器标准核心服务
/// </summary>
public interface IServerCoreStandardRegisterService : IXFEServerCoreService
{
    /// <summary>
    /// 服务器标准请求接收事件
    /// </summary>
    /// <param name="execute">执行语句</param>
    /// <param name="queryableJsonNode">json请求节点</param>
    /// <param name="r">返回参数</param>
    void StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r);
}
