namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 异步服务器服务接口
/// </summary>
public interface IAsyncServerService : IXFEServerService
{
    /// <summary>
    /// 启动服务（异步）
    /// </summary>
    /// <returns></returns>
    Task StartServiceAsync(string ipAddress);
}
