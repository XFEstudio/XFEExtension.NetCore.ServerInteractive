using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器
/// </summary>
[CreateImpl]
public class XFEServer
{
    readonly List<IServerInitializerService> serverInitializerServiceList = [];
    readonly List<IServerService> serverServiceList = [];
    readonly List<IAsyncServerService> asyncServerServiceList = [];
    /// <summary>
    /// 绑定的IP地址（服务初始化执行完成后才会刷新）
    /// </summary>
    public string BindingIPAddress { get; set; } = "http://localhost:8080/";

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        foreach (var serverInitializerService in serverInitializerServiceList)
            serverInitializerService.Initialize();
        foreach (var serverService in serverServiceList)
            serverService.StartService(BindingIPAddress);
        foreach (var asyncServerService in asyncServerServiceList)
            await asyncServerService.StartServiceAsync(BindingIPAddress);
    }
}