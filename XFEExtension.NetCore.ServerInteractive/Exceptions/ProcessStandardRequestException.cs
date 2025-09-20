namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// 处理标准请求时异常
/// </summary>
public class ProcessStandardRequestException : XFEServerCoreException
{
    /// <inheritdoc/>
    public ProcessStandardRequestException() { }
    /// <inheritdoc/>
    /// <inheritdoc/>
    public ProcessStandardRequestException(string? message) : base(message) { }
    /// <inheritdoc/>
    public ProcessStandardRequestException(string? message, Exception? innerException) : base(message, innerException) { }
}
