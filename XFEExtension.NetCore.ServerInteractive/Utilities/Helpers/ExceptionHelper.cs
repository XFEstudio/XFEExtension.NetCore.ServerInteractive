namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 异常帮助类
/// </summary>
public static class ExceptionHelper
{
    /// <summary>
    /// 获取异常消息及其内部异常消息
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="maxInnerExceptionDepth">最大内部异常层数</param>
    /// <returns>异常消息</returns>
    public static string GetExceptionMessage(Exception? exception, int maxInnerExceptionDepth = 5)
    {
        if (exception is null)
        {
            return string.Empty;
        }

        var errorMessage = exception.Message;
        var currentException = exception.InnerException;
        for (var i = 0; i < maxInnerExceptionDepth; i++)
        {
            if (currentException is null)
            {
                break;
            }

            errorMessage += $"：{currentException.Message}";
            currentException = currentException.InnerException;
        }

        return errorMessage;
    }
}
