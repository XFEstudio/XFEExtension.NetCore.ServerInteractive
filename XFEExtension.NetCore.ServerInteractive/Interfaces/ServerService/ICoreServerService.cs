namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 核心服务器服务
/// </summary>
public interface ICoreServerService : IXFEServerServiceBase
{
    /// <summary>
    /// 绑定的IP地址（服务初始化执行完成后才会刷新）
    /// </summary>
    string BindingIPAddress { get; set; }
    /// <summary>
    /// 核心服务器名称
    /// </summary>
    string CoreServerName { get; set; }
    /// <summary>
    /// 启动核心服务器
    /// </summary>
    /// <returns></returns>
    Task StartServerCore();
}
