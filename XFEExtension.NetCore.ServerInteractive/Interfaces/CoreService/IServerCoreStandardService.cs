namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器标准核心服务
/// </summary>
public interface IServerCoreStandardService : IXFEStandardServerCoreServiceBase
{
    /// <summary>
    /// 服务器标准核心服务初始化事件（在请求初始化的时候执行）
    /// </summary>
    void Initialize();
    /// <summary>
    /// 服务器标准请求接收异步事件
    /// 子类可直接通过属性访问 Execute / QueryableJsonNode / ReturnArgs
    /// </summary>
    Task RequestReceiveAsync();
    /// <summary>
    /// 服务器标准请求接收事件
    /// 子类可直接通过属性访问 Execute / QueryableJsonNode / ReturnArgs
    /// </summary>
    void RequestReceive();
}
