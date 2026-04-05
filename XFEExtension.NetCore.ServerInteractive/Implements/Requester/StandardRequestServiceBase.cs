using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Implements.Requester;

/// <summary>
/// 标准请求服务基类
/// </summary>
public abstract class StandardRequestServiceBase : IStandardRequestService
{
    /// <summary>
    /// 本类型的请求路由路径列表（由增量生成器自动填充）
    /// </summary>
    public static List<string> RequestRouteList { get; } = [];

    /// <inheritdoc/>
    public string Session { get => XFEClientRequester.Session; set => XFEClientRequester.Session = value; }
    /// <inheritdoc />
    public string Route { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string DeviceInfo { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string UnescapedResponse { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string Response { get; set; } = string.Empty;
    /// <inheritdoc/>
    public object[] Parameters { get; set; } = [];
    /// <inheritdoc/>
    public XFEClientRequester XFEClientRequester { get; set; }

    /// <inheritdoc/>
    public virtual Dictionary<string, Func<object>> RequestPoints { get; } = new();

    /// <inheritdoc/>
    public virtual Dictionary<string, Func<object>> ResponsePoints { get; } = new();

    /// <inheritdoc/>
    public virtual Dictionary<string, string> RequestRouteMap { get; } = new();
}
