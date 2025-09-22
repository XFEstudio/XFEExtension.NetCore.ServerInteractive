namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 核心服务器处理服务
/// </summary>
public interface ICoreServerProcessService : IXFEServerServiceBase
{
    /// <summary>
    /// 核心服务器列表
    /// </summary>
    IReadOnlyList<ICoreServerService> CoreServerServiceList { get; set; }
    /// <summary>
    /// 处理核心服务器
    /// </summary>
    /// <returns></returns>
    Task ProcessCoreServer();
}
