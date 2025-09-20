
namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// XFE服务器构建器异常
/// </summary>
public class XFEServerBuilderException : XFEServerException
{
    /// <inheritdoc/>
    public XFEServerBuilderException() { }
    /// <inheritdoc/>
    public XFEServerBuilderException(string? message) : base(message) { }
    /// <inheritdoc/>
    public XFEServerBuilderException(string? message, Exception? innerException) : base(message, innerException) { }
}
