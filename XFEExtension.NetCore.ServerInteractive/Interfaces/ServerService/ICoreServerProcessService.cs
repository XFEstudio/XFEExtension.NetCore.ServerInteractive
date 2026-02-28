namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 核心服务器处理服务
/// </summary>
public interface IServerCoreProcessService : IXFEServerServiceBase
{
    /// <summary>
    /// 核心服务器列表
    /// </summary>
    IReadOnlyList<IServerCoreService> ServerCoreServiceList { get; set; }
    /// <summary>
    /// 处理核心服务器
    /// </summary>
    /// <returns></returns>
    Task ProcessServerCore();
}
