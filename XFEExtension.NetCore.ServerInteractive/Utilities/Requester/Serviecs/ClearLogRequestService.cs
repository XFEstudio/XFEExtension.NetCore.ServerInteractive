using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 清除日志请求服务
/// </summary>
public class ClearLogRequestService : XFERequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => true;

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters) => new
    {
        execute,
        session = XFEClientRequester.Session,
        computerInfo = XFEClientRequester.ComputerInfo
    };
}
