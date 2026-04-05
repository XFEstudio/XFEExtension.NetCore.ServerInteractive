namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 提供标准核心服务路由路径列表的静态抽象契约
/// </summary>
public interface IServerCoreRouteProvider
{
    /// <summary>
    /// 入口点路径列表
    /// </summary>
    static abstract List<string> EntryPointList { get; }
}
