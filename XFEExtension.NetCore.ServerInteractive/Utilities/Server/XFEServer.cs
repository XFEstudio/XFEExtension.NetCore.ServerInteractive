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
    internal List<IServerInitializerService> serverInitializerServiceList = [];
    internal List<IServerService> serverServiceList = [];
    internal List<IAsyncServerService> asyncServerServiceList = [];
    /// <summary>
    /// 核心服务器处理服务
    /// </summary>
    public ICoreServerProcessService CoreServerProcessService { get; set; } = new CoreServerProcessServiceBaseImpl();

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        foreach (var serverInitializerService in serverInitializerServiceList)
            serverInitializerService.Initialize();
        foreach (var serverService in serverServiceList)
            serverService.StartService();
        foreach (var asyncServerService in asyncServerServiceList)
            await asyncServerService.StartServiceAsync();
        await CoreServerProcessService.ProcessCoreServer();
    }
}