using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;

/// <summary>
/// XFE服务器核心服务
/// </summary>
public interface IXFEServerCoreService
{
    /// <summary>
    /// 服务器核心
    /// </summary>
    XFEServerCore XFEServerCore { get; init; }
}