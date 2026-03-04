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
    public async Task Close(string message) => await ReturnArgs!.Close(message);

    /// <inheritdoc/>
    public void OK() => ReturnArgs?.OK();

    /// <inheritdoc/>
    public Task CloseWithError(string message, HttpStatusCode code) => ReturnArgs!.CloseWithError(message, code);

    /// <inheritdoc/>
    public ServerCoreReturnArgs Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest, bool handled = false) => ReturnArgs!.Error(message, code, handled);
}
