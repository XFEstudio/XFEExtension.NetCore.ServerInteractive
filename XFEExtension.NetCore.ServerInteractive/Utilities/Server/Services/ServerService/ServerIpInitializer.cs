using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// 服务器IP初始化服务
/// </summary>
[Obsolete("核心服务器现已不再使用单一IP，故IP初始化器不再提供", DiagnosticId = "XFW0002", UrlFormat = "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFW0002")]
public class ServerIPInitializer : ServerInitializerServiceBase
{
    private static int s_nextDefaultPort = 3300;
    /// <inheritdoc/>
    public override void Initialize()
    {
        ServerBaseProfile.SaveProfile();
        foreach (var serverCoreService in XFEServer.ServerCoreProcessService.ServerCoreServiceList)
        {
            ServerBaseProfile.ServerLastBindingAddressDictionary.TryAdd(serverCoreService.ServerCoreName, $"http://localhost:{s_nextDefaultPort++}/");
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
            serverCoreService.BindingIPAddressList = [];
            Console.WriteLine($"服务器{serverCoreService.ServerCoreName}设置完成！IP为：{ipAddress}");
        }
        ServerBaseProfile.SaveProfile();
    }
}
