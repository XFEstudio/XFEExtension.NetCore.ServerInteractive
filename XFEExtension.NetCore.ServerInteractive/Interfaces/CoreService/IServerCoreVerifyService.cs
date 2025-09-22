using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器核心初始化服务
/// </summary>
public interface IServerCoreVerifyService : IXFEServerCoreServiceBase
{
    /// <summary>
    /// 校验请求
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    bool VerifyRequest(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r);
}
