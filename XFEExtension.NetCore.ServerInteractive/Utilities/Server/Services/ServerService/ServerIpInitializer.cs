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
        foreach (var coreServerService in XFEServer.CoreServerProcessService.CoreServerServiceList)
        {
            ServerBaseProfile.ServerLastBindingAddressDictionary.TryAdd(coreServerService.CoreServerName, "http://localhost:8080/");
            if (ServerBaseProfile.ServerLastBindingAddressDictionary.TryGetValue(coreServerService.CoreServerName, out var ipAddress))
            {
                Console.WriteLine($"正在设置服务器：{coreServerService.CoreServerName}");
                Console.WriteLine($"是否绑定上一次IP：{ipAddress}？(Y/N)");
                if (Console.ReadKey().Key == ConsoleKey.N)
                {
                    ipAddress = string.Empty;
                    while (ipAddress.IsNullOrWhiteSpace())
                    {
                        Console.Write("请输入绑定的IP：");
                        ipAddress = Console.ReadLine();
                    }
                    ServerBaseProfile.ServerLastBindingAddressDictionary[coreServerService.CoreServerName] = ipAddress;
                }
                coreServerService.BindingIPAddress = ipAddress;
                Console.WriteLine($"服务器{coreServerService.CoreServerName}设置完成！IP为：{ipAddress}");
            }
        }
    }
}
