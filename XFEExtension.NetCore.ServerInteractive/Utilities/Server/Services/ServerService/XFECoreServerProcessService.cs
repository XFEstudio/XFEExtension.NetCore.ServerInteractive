using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// XFE核心服务器处理程序
/// </summary>
public class XFEServerCoreProcessService : ServerCoreProcessServiceBase
{
    /// <inheritdoc/>
    public override async Task ProcessServerCore()
    {
        while (true)
        {
            try
            {
                Console.WriteLine(File.Exists("server.log") ? "[DEBUG]找到日志文件！" : "[DEBUG]未找到日志文件！");
                var taskList = ServerCoreServiceList.Select(serverCoreService => Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                Console.WriteLine($"[DEBUG]正在启动服务器：{serverCoreService.ServerCoreName}({serverCoreService.BindingIPAddress})...");
                                await serverCoreService.StartServerCore();
                                Console.WriteLine($"[ERROR]服务器({serverCoreService.ServerCoreName})因未知原因自行结束！");
                                Console.WriteLine("[DEBUG]准备重启服务器({serverCoreService.ServerCoreName})");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 3...");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 2...");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 1...");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR]服务器({serverCoreService.ServerCoreName})错误：{ex.Message}：{ex.InnerException?.Message}");
                                Console.WriteLine($"[TRACE]{ex.StackTrace}");
                                Console.WriteLine($"[DEBUG]准备重启服务器({serverCoreService.ServerCoreName})");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 3...");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 2...");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({serverCoreService.ServerCoreName}) 1...");
                            }
                        }
                    }))
                    .ToList();
                await Task.WhenAll(taskList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG]服务器错误：{ex}");
                Console.WriteLine($"[ERROR]服务器错误：{ex.Message}");
                Console.WriteLine($"[TRACE]{ex.StackTrace}");
                Console.WriteLine("[DEBUG]准备重启服务器");
                await Task.Delay(1000);
                Console.WriteLine("[DEBUG]正在重启服务器 3...");
                await Task.Delay(1000);
                Console.WriteLine("[DEBUG]正在重启服务器 2...");
                await Task.Delay(1000);
                Console.WriteLine("[DEBUG]正在重启服务器 1...");
            }
        }
    }
}