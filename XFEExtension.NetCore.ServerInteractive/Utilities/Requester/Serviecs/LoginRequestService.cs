using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 登录请求服务
/// </summary>
public class LoginRequestService : XFERequestServiceBase
{
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response)
    {
        QueryableJsonNode jsonNode = response;
        string session = jsonNode["session"];
        XFEClientRequester.Session = session;
        return (session, DateTime.Parse(jsonNode["expireDate"]));
    }

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters) => new
    {
        execute,
        computerInfo = XFEClientRequester.ComputerInfo,
        account = parameters[0],
        password = parameters[1]
    };
}
