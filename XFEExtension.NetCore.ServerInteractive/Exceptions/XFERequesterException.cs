using System.Runtime.Serialization;

namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// XFE请求器异常
/// </summary>
public class XFERequesterException : Exception
{
    /// <inheritdoc/>
    public XFERequesterException() { }
    /// <inheritdoc/>
    public XFERequesterException(string? message) : base(message) { }
    /// <inheritdoc/>
    public XFERequesterException(string? message, Exception? innerException) : base(message, innerException) { }
}
