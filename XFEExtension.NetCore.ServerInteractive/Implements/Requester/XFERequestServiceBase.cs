using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Implements.Requester;

/// <summary>
/// XFE请求服务基类
/// </summary>
public abstract class XFERequestServiceBase : IXFERequestService
{
    /// <inheritdoc/>
    public string Session { get => XFEClientRequester.Session; set => XFEClientRequester.Session = value; }
    /// <inheritdoc/>
    public string Execute { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string DeviceInfo { get; set; } = string.Empty;
    /// <inheritdoc/>
    public object[] Parameters { get; set; } = [];
    /// <inheritdoc/>
    public XFEClientRequester XFEClientRequester { get; set; }
    /// <inheritdoc/>
    public abstract object AnalyzeResponse(string response);

    /// <inheritdoc/>
    public abstract object PostRequest();
}
