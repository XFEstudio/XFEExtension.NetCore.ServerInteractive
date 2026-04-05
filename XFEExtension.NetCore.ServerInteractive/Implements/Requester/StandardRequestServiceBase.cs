using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Implements.Requester;

/// <summary>
/// 标准请求服务基类
/// </summary>
public abstract class StandardRequestServiceBase : IStandardRequestService
{
    /// <summary>
    /// 此服务对应的路由列表（由派生类定义）
    /// </summary>
    public static List<string> RouteList { get; } = new();

    /// <inheritdoc/>
    public string Session { get => XFEClientRequester.Session; set => XFEClientRequester.Session = value; }
    /// <inheritdoc/>
    public string Execute { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string DeviceInfo { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string UnescapedResponse { get; set; }
    /// <inheritdoc/>
    public string Response { get; set; }
    /// <inheritdoc/>
    public object[] Parameters { get; set; } = [];
    /// <inheritdoc/>
    public XFEClientRequester XFEClientRequester { get; set; }

    /// <inheritdoc/>
    public abstract object AnalyzeResponse();

    /// <inheritdoc/>
    public abstract object PostRequest();
}
