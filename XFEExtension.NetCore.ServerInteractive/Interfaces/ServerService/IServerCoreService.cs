namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 核心服务器服务
/// </summary>
public interface IServerCoreService : IXFEServerServiceBase
{
    /// <summary>
    /// 绑定的IP地址（服务初始化执行完成后才会刷新）
    /// </summary>
    string BindingIpAddress { get; set; }
    /// <summary>
    /// 核心服务器名称
    /// </summary>
    string ServerCoreName { get; set; }
    /// <summary>
    /// 启动核心服务器
    /// </summary>
    /// <returns></returns>
    Task StartServerCore();
}
