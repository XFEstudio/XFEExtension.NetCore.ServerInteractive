using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.ServerService;

/// <summary>
/// 服务器异常处理服务
/// </summary>
public class ServerExceptionProcessService : ServerServiceBase
{
    /// <inheritdoc/>
    public override void StartService()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        Console.WriteLine("[DEBUG]正在关闭服务器...");
        Console.WriteLine("[DEBUG]正在保存日志...");
        XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
        Console.WriteLine("[DEBUG]日志已保存！");
        Console.WriteLine("[DEBUG]服务器已关闭！");
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine($"[FATAL]{e.ExceptionObject}");
        if (e.ExceptionObject is Exception exception)
            Console.WriteLine($"[TRACE]{exception.StackTrace}");
        XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
    }
}
