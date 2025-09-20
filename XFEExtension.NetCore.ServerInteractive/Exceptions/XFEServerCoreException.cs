namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// XFE服务器核心异常
/// </summary>
public class XFEServerCoreException : Exception
{
    /// <inheritdoc/>
    public XFEServerCoreException() { }
    /// <inheritdoc/>
    public XFEServerCoreException(string? message) : base(message) { }
    /// <inheritdoc/>
    public XFEServerCoreException(string? message, Exception? innerException) : base(message, innerException) { }
}
