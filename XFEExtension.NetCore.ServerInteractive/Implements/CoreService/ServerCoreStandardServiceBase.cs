using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract partial class ServerCoreStandardServiceBase : XFEServerCoreServiceBase, IServerCoreStandardService
{
    /// <inheritdoc/>
    public virtual Dictionary<string, Action> SyncEntryPoints { get; } = new();

    /// <inheritdoc/>
    public virtual Dictionary<string, Func<Task>> AsyncEntryPoints { get; } = new();

    /// <inheritdoc/>
    public virtual void Initialize() { }

    /// <summary>
    /// 服务器标准请求接收事件（已废弃，请使用EntryPointAttribute特性标记方法）
    /// </summary>
    [Obsolete("此方法已废弃，请使用EntryPointAttribute特性标记方法来定义次级入口点")]
    public virtual void RequestReceive() { }

    /// <summary>
    /// 服务器标准请求接收异步事件（已废弃，请使用EntryPointAttribute特性标记方法）
    /// </summary>
    [Obsolete("此方法已废弃，请使用EntryPointAttribute特性标记方法来定义次级入口点")]
    public virtual Task RequestReceiveAsync() => Task.CompletedTask;
}
