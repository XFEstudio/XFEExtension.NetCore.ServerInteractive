using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器
/// </summary>
[CreateImpl]
public class XFEServer
{
    internal readonly List<IServerInitializerService> ServerInitializerServiceList = [];
    internal readonly List<IServerService> ServerServiceList = [];
    internal readonly List<IAsyncServerService> AsyncServerServiceList = [];
    /// <summary>
    /// 核心服务器处理服务
    /// </summary>
    public IServerCoreProcessService ServerCoreProcessService { get; set; } = new ServerCoreProcessServiceBaseImpl();

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        foreach (var serverInitializerService in ServerInitializerServiceList)
            serverInitializerService.Initialize();
        foreach (var serverService in ServerServiceList)
            serverService.StartService();
        foreach (var asyncServerService in AsyncServerServiceList)
            await asyncServerService.StartServiceAsync();
        await ServerCoreProcessService.ProcessServerCore();
    }
}