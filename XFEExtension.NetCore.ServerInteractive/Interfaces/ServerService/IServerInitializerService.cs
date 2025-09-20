namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// 服务器初始化服务
/// </summary>
public interface IServerInitializerService : IXFEServerService
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Initialize();
}
