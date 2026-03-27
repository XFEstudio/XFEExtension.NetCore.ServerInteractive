namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// XFE请求服务
/// </summary>
public interface IXFERequestService : IRequestServiceBase
{
    /// <summary>
    /// 待请求的方法（如 check_connect）
    /// </summary>
    string Execute { get; set; }
    /// <summary>
    /// 设备信息
    /// </summary>
    string DeviceInfo { get; set; }
    /// <summary>
    /// 待请求的方法的参数列表
    /// </summary>
    object[] Parameters { get; set; }
    /// <summary>
    /// 提交请求体
    /// </summary>
    /// <returns></returns>
    object PostRequest();
    /// <summary>
    /// 接收到返回后解析
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    object AnalyzeResponse(string response);
}
