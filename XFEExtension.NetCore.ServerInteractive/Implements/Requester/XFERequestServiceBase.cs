using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Implements.Requester;

/// <summary>
/// XFE请求服务基类
/// </summary>
public abstract class XFERequestServiceBase : IXFERequestService
{
    /// <inheritdoc/>
    public XFEClientRequester XFEClientRequester { get; set; }

    /// <inheritdoc/>
    public abstract object AnalyzeResponse(string response);
    /// <inheritdoc/>
    public abstract object PostRequest(string execute, params object[] parameters);
}
