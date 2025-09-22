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
    /// <param name="parameters"></param>
    /// <returns></returns>
    object PostRequest(params object[] parameters);
    /// <summary>
    /// 接收到返回后解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    Task<T> AnalyzeResponse<T>(string response);
}
