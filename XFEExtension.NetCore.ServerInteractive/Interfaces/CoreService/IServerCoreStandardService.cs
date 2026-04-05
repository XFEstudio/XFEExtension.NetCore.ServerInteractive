namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器标准核心服务
/// </summary>
public interface IServerCoreStandardService : IXFEStandardServerCoreServiceBase
{
    /// <summary>
    /// 同步入口点字典，Key为入口点路径，Value为对应的处理方法
    /// </summary>
    Dictionary<string, Action> SyncEntryPoints { get; }

    /// <summary>
    /// 异步入口点字典，Key为入口点路径，Value为对应的异步处理方法
    /// </summary>
    Dictionary<string, Func<Task>> AsyncEntryPoints { get; }

    /// <summary>
    /// 服务器标准核心服务初始化事件（在请求初始化的时候执行）
    /// </summary>
    void Initialize();
}
