using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 核心服务器异常处理服务
/// </summary>
public class ServerCoreExceptionProcessService : ServerCoreOriginalServiceBase
{
    /// <inheritdoc/>
    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e) { }

    /// <inheritdoc/>
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine($"服务器({XFEServerCore.ServerCoreName})已启动！");
        XFEServerCore.ServerCoreError += XFEServerCore_ServerCoreError;
    }

    private static async void XFEServerCore_ServerCoreError(XFEServerCore sender, ServerCoreErrorEventArgs e)
    {
        try
        {
            if (e.Handled || e.ReturnArgs is null) return;
            var currentException = e.ServerException?.InnerException;
            var errorMessage = e.ServerException?.Message ?? "服务器内部异常";
            var errorInfo = string.Empty;
            if (e.ServerException?.InnerException is ServerCoreReturnArgs returnArgs)
                errorInfo = returnArgs.ReturnMessage;
            if (e.ReturnArgs.IsStandardError)
            {
                Console.Write($"\t【{errorInfo}】");
            }
            else
            {
                for (var i = 0; i < 5; i++)
                {
                    if (currentException is null)
                    {
                        break;
                    }
                    errorMessage += $"：{currentException.Message}";
                    currentException = currentException.InnerException;
                }
                errorInfo = errorMessage.Contains("请求的路由未注册") ? "路由未找到" : "服务器内部异常";
                Console.WriteLine();
                Console.WriteLine($"[WARN]【{errorInfo}】{errorMessage}");
                if (e.ServerException?.InnerException?.StackTrace is not null)
                    Console.WriteLine($"[TRACE]{e.ServerException?.InnerException?.StackTrace}");
            }

            try
            {
                await e.ReturnArgs.CloseWithError(errorInfo, e.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]发送错误响应时发生异常：{ex.Message}");
                if (ex.StackTrace is not null)
                    Console.WriteLine($"[TRACE]{ex.StackTrace}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]处理服务器异常时发生异常：{ex.Message}");
            if (ex.StackTrace is not null)
                Console.WriteLine($"[TRACE]{ex.StackTrace}");
        }
    }
}
