using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 获取日志请求服务
/// </summary>
public class GetLogRequestService : XFERequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => response;

    /// <inheritdoc/>
    public override object PostRequest() => new
    {
        execute = Execute,
        session = Session,
        deviceInfo = DeviceInfo,
        startDateTime = Parameters[0],
        endDateTime = Parameters[1],
    };
}
