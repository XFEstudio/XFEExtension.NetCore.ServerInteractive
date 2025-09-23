using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 获取日志请求服务
/// </summary>
public class GetLogRequestService : XFERequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => response;

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters) => new
    {
        execute,
        session = XFEClientRequester.Session,
        computerInfo = XFEClientRequester.ComputerInfo,
        startDateTime = parameters[0],
        endDateTime = parameters[1],
    };
}
