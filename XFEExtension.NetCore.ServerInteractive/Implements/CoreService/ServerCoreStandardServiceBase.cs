using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardServiceBase : XFEServerCoreServiceBase, IServerCoreStandardService
{
    /// <summary>
    /// 处理请求（无参，子类直接使用属性）
    /// </summary>
    public abstract void StandardRequestReceived();
}
