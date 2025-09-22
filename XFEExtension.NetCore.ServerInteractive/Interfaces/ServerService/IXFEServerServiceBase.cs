using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

/// <summary>
/// XFE服务器服务接口
/// </summary>
public interface IXFEServerServiceBase
{
    /// <summary>
    /// XFE服务器
    /// </summary>
    XFEServer XFEServer { get; set; }
}
