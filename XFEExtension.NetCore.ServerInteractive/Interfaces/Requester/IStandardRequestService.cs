namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 标准请求服务
/// </summary>
public interface IStandardRequestService : IRequestServiceBase
{
    /// <summary>
    /// 设备信息
    /// </summary>
    string DeviceInfo { get; internal set; }
    /// <summary>
    /// 当前会话ID
    /// </summary>
    string Session { get; set; }
    /// <summary>
    /// 请求路由（不带斜杠开头），如"user/relogin"
    /// </summary>
    string Route { get; set; }
    /// <summary>
    /// 返回结果字符串（转义后）
    /// </summary>
    string UnescapedResponse { get; set; }
    /// <summary>
    /// 返回结果字符串
    /// </summary>
    string Response { get; set;  }
    /// <summary>
    /// 待请求的方法的参数列表
    /// </summary>
    object[] Parameters { get; internal set; }
    /// <summary>
    /// 提交请求体
    /// </summary>
    /// <returns></returns>
    object PostRequest();
    /// <summary>
    /// 接收到返回后解析
    /// </summary>
    /// <returns></returns>
    object AnalyzeResponse();
}
