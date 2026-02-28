using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// 服务器IP初始化服务
/// </summary>
public class ServerIpInitializer : ServerInitializerServiceBase
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        foreach (var serverCoreService in XFEServer.ServerCoreProcessService.ServerCoreServiceList)
        {
            ServerBaseProfile.ServerLastBindingAddressDictionary.TryAdd(serverCoreService.ServerCoreName, "http://localhost:8080/");
            if (ServerBaseProfile.ServerLastBindingAddressDictionary.TryGetValue(serverCoreService.ServerCoreName, out var ipAddress))
            {
                Console.WriteLine($"正在设置服务器：{serverCoreService.ServerCoreName}");
                Console.WriteLine($"是否绑定上一次IP：{ipAddress}？(Y/N)");
                var key = Console.ReadKey();
                Console.WriteLine(key.Key.ToString());
                if (key.Key == ConsoleKey.N)
                {
                    ipAddress = string.Empty;
                    while (ipAddress.IsNullOrWhiteSpace())
                    {
                        Console.Write("请输入绑定的IP：");
                        ipAddress = Console.ReadLine();
                    }
                    ServerBaseProfile.ServerLastBindingAddressDictionary[serverCoreService.ServerCoreName] = ipAddress;
                }
                serverCoreService.BindingIPAddress = ipAddress;
                Console.WriteLine($"服务器{serverCoreService.ServerCoreName}设置完成！IP为：{ipAddress}");
            }
        }
        ServerBaseProfile.SaveProfile();
    }
}
