using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 清除日志请求服务
/// </summary>
public partial class ClearLogRequestService : StandardRequestServiceBase
{
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
