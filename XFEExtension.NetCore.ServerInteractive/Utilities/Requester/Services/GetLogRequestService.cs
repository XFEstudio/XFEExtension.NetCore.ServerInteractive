using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 获取日志请求服务
/// </summary>
public partial class GetLogRequestService : StandardRequestServiceBase
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
}
