using System.Net;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器核心服务基类
/// </summary>
public abstract class XFEServerCoreServiceBase : IXFEServerCoreServiceBase
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; }
    /// <inheritdoc/>
    public string Execute { get; set; } = string.Empty;
    /// <inheritdoc/>
    public QueryableJsonNode Json { get; set; }
    /// <inheritdoc/>
    public ServerCoreReturnArgs ReturnArgs { get; set; }

    /// <inheritdoc/>
    public async Task Close(string message) => await ReturnArgs!.Args.ReplyAndClose(message);

    /// <inheritdoc/>
    public void Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest) => ReturnArgs?.Error(message, code);

    /// <inheritdoc/>
    public void OK() => ReturnArgs?.Close(HttpStatusCode.OK);
}
