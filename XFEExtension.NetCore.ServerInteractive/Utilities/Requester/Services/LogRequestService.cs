using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 日志请求服务（包含获取日志与清除日志）
/// </summary>
public partial class LogRequestService : StandardRequestServiceBase
{
    /// <summary>
    /// 解析获取日志响应
    /// </summary>
    [Response("log/get")]
    public object AnalyzeGetLogResponse() => Response;

    /// <summary>
    /// 构造获取日志请求体
    /// </summary>
    [Request("log/get")]
    public object PostGetLogRequest() => new
    {
        session = Session,
        deviceInfo = DeviceInfo,
        startDateTime = Parameters[0],
        endDateTime = Parameters[1],
    };

    /// <summary>
    /// 解析清除日志响应
    /// </summary>
    [Response("log/clear")]
    public object AnalyzeClearLogResponse() => true;

    /// <summary>
    /// 构造清除日志请求体
    /// </summary>
    [Request("log/clear")]
    public object PostClearLogRequest() => new
    {
        session = Session,
        deviceInfo = DeviceInfo
    };
}
