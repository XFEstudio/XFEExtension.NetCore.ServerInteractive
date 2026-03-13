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

    private async void XFEServerCore_ServerCoreError(XFEServerCore sender, ServerCoreErrorEventArgs e)
    {
        if (!e.Handled && e.ReturnArgs is not null)
        {
            var currentException = e.ServerException?.InnerException;
            var errorMessage = e.ServerException?.Message ?? "服务器内部异常";
            var errorInfo = "服务器内部异常";
            if (e.ServerException?.InnerException is ServerCoreReturnArgs returnArgs)
                errorInfo = returnArgs.ReturnMessage;
            for (int i = 0; i < 5; i++)
            {
                if (currentException is null)
                {
                    break;
                }
                else
                {
                    errorMessage += $"：{currentException.Message}";
                    currentException = currentException.InnerException;
                }
            }
            Console.WriteLine();
            Console.WriteLine($"[WARN]【{errorInfo}】{errorMessage}");
            if (e.ServerException?.InnerException?.StackTrace is not null)
                Console.WriteLine($"[TRACE]{e.ServerException?.InnerException?.StackTrace}");
            try
            {
                await e.ReturnArgs.CloseWithError(errorInfo, e.StatusCode);
            }
            catch { }
        }
    }
}
