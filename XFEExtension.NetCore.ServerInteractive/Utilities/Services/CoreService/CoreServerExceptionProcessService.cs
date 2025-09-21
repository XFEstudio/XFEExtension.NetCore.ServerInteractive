using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <summary>
/// 核心服务器异常处理服务
/// </summary>
public class CoreServerExceptionProcessService : ServerCoreRegisterServiceBase
{
    /// <inheritdoc/>
    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e) { }

    /// <inheritdoc/>
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine($"服务器({XFEServerCore.CoreServerName})已启动！");
        XFEServerCore.ServerCoreError += XFEServerCore_ServerCoreError;
    }

    private async void XFEServerCore_ServerCoreError(Server.XFEServerCore sender, Models.ServerCoreErrorEventArgs e)
    {
        if (!e.Handled && e.CyberCommRequestEventArgs is not null)
        {
            Console.WriteLine($"[WARN]【{e.CyberCommRequestEventArgs.ClientIP}】{e.ServerException?.Message}");
            if (e.ServerException?.StackTrace is not null)
                Console.WriteLine($"[TRACE]{e.ServerException?.StackTrace}");
            await e.CyberCommRequestEventArgs.ReplyAndClose(e.ServerException?.Message ?? "服务器内部异常", e.StatusCode);
        }
    }
}
