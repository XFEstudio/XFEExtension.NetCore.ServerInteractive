namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// XFE服务器异常
/// </summary>
public class XFEServerException : Exception
{
    /// <inheritdoc/>
    public XFEServerException() { }
    /// <inheritdoc/>
    public XFEServerException(string? message) : base(message) { }
    /// <inheritdoc/>
    public XFEServerException(string? message, Exception? innerException) : base(message, innerException) { }
}
