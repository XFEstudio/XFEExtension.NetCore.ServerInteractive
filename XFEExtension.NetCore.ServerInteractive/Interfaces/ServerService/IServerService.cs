namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 服务器服务接口
/// </summary>
public interface IServerService : IXFEServerService
{
    /// <summary>
    /// 启动服务
    /// </summary>
    void StartService(string ipAddress);
}