using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// 服务器IP初始化服务
/// </summary>
public class ServerIpInitializer : ServerInitializerServiceBase
{
    private static int _nextDefaultPort = 3300;
    /// <inheritdoc/>
    public override void Initialize()
    {
        ServerBaseProfile.SaveProfile();
        foreach (var serverCoreService in XFEServer.ServerCoreProcessService.ServerCoreServiceList)
        {
            ServerBaseProfile.ServerLastBindingAddressDictionary.TryAdd(serverCoreService.ServerCoreName, $"http://localhost:{_nextDefaultPort++}/");
            if (!ServerBaseProfile.ServerLastBindingAddressDictionary.TryGetValue(serverCoreService.ServerCoreName, out var ipAddress)) continue;
            Console.WriteLine($"正在设置服务器：{serverCoreService.ServerCoreName}");
            Console.WriteLine($"是否绑定IP：{ipAddress}？(Y/N)");
            var key = Console.ReadKey();
            Console.WriteLine(key.Key.ToString());
            if (key.Key == ConsoleKey.N)
            {
                ipAddress = string.Empty;
                while (ipAddress.IsNullOrWhiteSpace())
                {
                    XFEConsole.XFEConsole.CurrentConsoleTextWriter?.OriginalTextWriter.Write("请输入绑定的IP：");
                    ipAddress = Console.ReadLine();
                }
                ServerBaseProfile.ServerLastBindingAddressDictionary[serverCoreService.ServerCoreName] = ipAddress;
            }
            serverCoreService.BindingIpAddress = ipAddress;
            Console.WriteLine($"服务器{serverCoreService.ServerCoreName}设置完成！IP为：{ipAddress}");
        }
        ServerBaseProfile.SaveProfile();
    }
}
