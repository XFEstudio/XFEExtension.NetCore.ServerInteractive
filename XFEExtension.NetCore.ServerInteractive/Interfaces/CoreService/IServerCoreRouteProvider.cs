namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 提供标准核心服务路由路径列表的实例路由提供者契约
/// </summary>
public interface IServerCoreRouteProvider
{
    /// <summary>
    /// 入口点路径列表
    /// </summary>
    List<string> EntryPointList { get; }
}
