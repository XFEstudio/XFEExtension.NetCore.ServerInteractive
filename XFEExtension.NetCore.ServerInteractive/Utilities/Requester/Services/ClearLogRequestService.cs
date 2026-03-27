using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 清除日志请求服务
/// </summary>
public class ClearLogRequestService : StandardRequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => true;

    /// <inheritdoc/>
    public override object PostRequest() => new
    {
        execute = Execute,
        session = Session,
        deviceInfo = DeviceInfo
    };
}
