namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// XFE请求服务
/// </summary>
public interface IXFERequestService : IRequestServiceBase
{
    /// <summary>
    /// 获取Session方法
    /// </summary>
    Func<string> GetSession { get; set; }
    /// <summary>
    /// 获取电脑信息方法
    /// </summary>
    Func<string> GetComputerInfo { get; set; }
    /// <summary>
    /// 提交请求体
    /// </summary>
    /// <param name="execute">执行的操作名称</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    object PostRequest(string execute, params object[] parameters);
    /// <summary>
    /// 接收到返回后解析
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    object AnalyzeResponse(string response);
}
