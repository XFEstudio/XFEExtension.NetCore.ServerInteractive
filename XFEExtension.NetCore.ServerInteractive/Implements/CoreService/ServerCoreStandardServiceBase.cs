using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract partial class ServerCoreStandardServiceBase : XFEServerCoreServiceBase, IServerCoreStandardService
{
    /// <summary>
    /// 所有入口点路径列表（由增量生成器自动填充）
    /// </summary>
    public static List<string> EntryPointList { get; } = [];

    /// <inheritdoc/>
    public virtual Dictionary<string, Action> SyncEntryPoints { get; } = new();

    /// <inheritdoc/>
    public virtual Dictionary<string, Func<Task>> AsyncEntryPoints { get; } = new();

    /// <inheritdoc/>
    public virtual void Initialize() { }
}
