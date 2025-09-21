using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.ServerService;

/// <summary>
/// XFE核心服务器处理程序
/// </summary>
public class XFECoreServerProcessService : CoreServerProcessServiceBase
{
    /// <inheritdoc/>
    public override async Task ProcessCoreServer()
    {
        while (true)
        {
            try
            {
                XFEConsole.XFEConsole.Log.Clear();
                Console.WriteLine("[DEBUG]加载日志文件...");
                if (File.Exists("server.log"))
                {
                    XFEConsole.XFEConsole.Log.Import("server.log".ReadOut()!);
                    Console.WriteLine($"[DEBUG]成功加载{XFEConsole.XFEConsole.Log.Logs.Count}条日志！");
                }
                else
                {
                    Console.WriteLine("[DEBUG]未找到日志文件！");
                }
                var taskList = new List<Task>();
                foreach (var coreServerService in CoreServerServiceList)
                {
                    taskList.Add(Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                Console.WriteLine($"[DEBUG]正在启动服务器：{coreServerService.CoreServerName}({coreServerService.BindingIPAddress})...");
                                await coreServerService.StartServerCore();
                                Console.WriteLine($"[ERROR]服务器({coreServerService.CoreServerName})因未知原因自行结束！");
                                Console.WriteLine("[DEBUG]准备重启服务器({coreServerService.CoreServerName})");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 3...");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 2...");
                                await Task.Delay(1000);
                                Console.WriteLine("[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 1...");
                                Console.WriteLine("[DEBUG]正在保存日志...");
                                XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
                                Console.WriteLine("[DEBUG]日志已保存！");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR]服务器({coreServerService.CoreServerName})错误：{ex.Message}");
                                Console.WriteLine($"[TRACE]{ex.StackTrace}");
                                Console.WriteLine($"[DEBUG]准备重启服务器({coreServerService.CoreServerName})");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 3...");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 2...");
                                await Task.Delay(1000);
                                Console.WriteLine($"[DEBUG]正在重启服务器({coreServerService.CoreServerName}) 1...");
                                Console.WriteLine("[DEBUG]正在保存日志...");
                                XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
                                Console.WriteLine("[DEBUG]日志已保存！");
                            }
                        }
                    }));
                }
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
                Console.WriteLine("[DEBUG]正在保存日志...");
                XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
                Console.WriteLine("[DEBUG]日志已保存！");
            }
        }
    }
}
