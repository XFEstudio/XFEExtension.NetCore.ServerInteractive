namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// 服务器核心初始化服务
/// </summary>
public interface IServerCoreVerifyService : IXFEStandardServerCoreServiceBase
{
    /// <summary>
    /// 校验请求
    /// </summary>
    /// <returns>如果请求通过校验，返回true；否则返回false。</returns>
    bool VerifyRequest();

    /// <summary>
    /// 校验请求
    /// </summary>
    /// <returns>如果请求通过校验，返回true；否则返回false。</returns>
    Task<bool> VerifyRequestAsync();
}
