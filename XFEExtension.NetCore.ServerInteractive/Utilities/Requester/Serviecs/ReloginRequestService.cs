using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 登录校验请求服务
/// </summary>
public class ReloginRequestService : XFERequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters)
    {

    }
}
