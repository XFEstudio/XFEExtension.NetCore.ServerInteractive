using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// 服务器IP初始化服务
/// </summary>
[Obsolete("核心服务器现已不再使用单一IP，故IP初始化器不再提供", DiagnosticId = "XFW0002", UrlFormat = "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFW0002")]
public class ServerIPInitializer : ServerInitializerServiceBase
{
    /// <inheritdoc/>
    public override void Initialize() { }
}
