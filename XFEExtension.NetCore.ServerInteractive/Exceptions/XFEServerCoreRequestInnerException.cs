
namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// XFE服务器核心请求内补异常
/// </summary>
public class XFEServerCoreRequestInnerException : XFEServerCoreException
{
    /// <inheritdoc/>
    public XFEServerCoreRequestInnerException() { }
    /// <inheritdoc/>
    public XFEServerCoreRequestInnerException(string? message) : base(message) { }
    /// <inheritdoc/>
    public XFEServerCoreRequestInnerException(string? message, Exception? innerException) : base(message, innerException) { }
}
