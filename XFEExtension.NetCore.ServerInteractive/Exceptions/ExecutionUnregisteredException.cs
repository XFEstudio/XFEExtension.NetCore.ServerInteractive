
namespace XFEExtension.NetCore.ServerInteractive.Exceptions;

/// <summary>
/// 执行的方法未注册异常
/// </summary>
public class ExecutionUnregisteredException : ProcessStandardRequestException
{
    /// <inheritdoc/>
    public ExecutionUnregisteredException()
    {
    }
    /// <inheritdoc/>
    public ExecutionUnregisteredException(string? message) : base(message)
    {
    }
    /// <inheritdoc/>
    public ExecutionUnregisteredException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
