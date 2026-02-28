namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器标准核心异步服务
/// </summary>
public interface IServerCoreStandardRegisterAsyncService : IXFEServerCoreServiceBase
{
    /// <summary>
    /// 服务器标准请求接收事件
    /// 子类可直接通过属性访问 Execute / QueryableJsonNode / ReturnArgs
    /// </summary>
    Task StandardRequestReceived();
}
