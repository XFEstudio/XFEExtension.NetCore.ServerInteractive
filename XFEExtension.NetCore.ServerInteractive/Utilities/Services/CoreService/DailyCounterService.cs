using System.Timers;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <summary>
/// 核心服务器天数服务
/// </summary>
public class DailyCounterService : ServerCoreRegisterServiceBase
{
    private int runningDays = 0;
    private DateTime startTime;
    /// <inheritdoc/>
    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e) { }

    /// <inheritdoc/>
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine("[DEBUG]天数统计服务已启动！");
        startTime = DateTime.Now;
        runningDays = 0;
        var serverTimer = new System.Timers.Timer(TimeSpan.FromHours(24))
        {
            AutoReset = true
        };
        serverTimer.Elapsed += ServerTimer_Elapsed;
        serverTimer.Start();
    }

    private void ServerTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        runningDays++;
        Console.WriteLine($"[DEBUG]自上次（{startTime}）启动以来，服务器已稳定运行 {runningDays}天");
        try
        {
            Console.WriteLine("[DEBUG]正在保存日志...");
            XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
            Console.WriteLine("[DEBUG]日志已保存！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]保存日志时发生错误：{ex.Message}");
            Console.WriteLine($"[TRACE]{ex.StackTrace}");
        }
    }
}
