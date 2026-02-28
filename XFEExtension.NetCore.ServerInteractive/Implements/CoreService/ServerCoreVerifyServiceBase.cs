using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.XFETransform.JsonConverter;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心校验服务基类
/// </summary>
public abstract class ServerCoreVerifyServiceBase : IServerCoreVerifyService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; }
    /// <inheritdoc/>
    public string Execute { get; set; } = string.Empty;
    /// <inheritdoc/>
    public QueryableJsonNode? QueryableJsonNode { get; set; }
    /// <inheritdoc/>
    public ServerCoreReturnArgs? ReturnArgs { get; set; }
    /// <inheritdoc/>
    public void OK() => ReturnArgs?.Close(HttpStatusCode.OK);
    /// <inheritdoc/>
    public void Close(string message, HttpStatusCode code = HttpStatusCode.InternalServerError) => ReturnArgs?.Error(message, code);
    /// <inheritdoc/>
    public abstract bool VerifyRequest(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r);
}
