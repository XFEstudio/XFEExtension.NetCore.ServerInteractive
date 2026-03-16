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
    public bool Handled { get => ReturnArgs.Handled; set => ReturnArgs.Handled = value; }
    /// <inheritdoc/>
    public string ClientIP { get => ReturnArgs.ClientIP; set => ReturnArgs.ClientIP = value; }
    /// <inheritdoc/>
    public bool IsStandardError { get => ReturnArgs.IsStandardError; set => ReturnArgs.IsStandardError = value; }
    /// <inheritdoc/>
    public string Execute { get; set; } = string.Empty;
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; }
    /// <inheritdoc/>
    public QueryableJsonNode Json { get; set; }
    /// <inheritdoc/>
    public ServerCoreReturnArgs ReturnArgs { get; set; }

    /// <inheritdoc/>
    public async Task Send(string message) => await ReturnArgs.Send(message);

    /// <inheritdoc/>
    public async Task Send(byte[] buffer) => await ReturnArgs.Send(buffer);

    /// <inheritdoc/>
    public async Task Send(Stream stream) => await ReturnArgs.Send(stream);

    /// <inheritdoc/>
    public async Task Close(string message) => await ReturnArgs.Close(message);

    /// <inheritdoc/>
    public async Task Close(byte[] buffer) => await ReturnArgs.Close(buffer);

    /// <inheritdoc/>
    public async Task Close(Stream stream) => await ReturnArgs.Close(stream);

    /// <inheritdoc/>
    public void OK() => ReturnArgs?.OK();

    /// <inheritdoc/>
    public async Task CloseWithError(string message, HttpStatusCode code) => await ReturnArgs!.CloseWithError(message, code);

    /// <inheritdoc/>
    public ServerCoreReturnArgs Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest, bool handled = false) => ReturnArgs!.Error(message, code, handled);

    /// <inheritdoc/>
    public async Task Send(object data) => await ReturnArgs.Send(data);

    /// <inheritdoc/>
    public async Task Close(object data) => await ReturnArgs.Close(data);
}
