using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

/// <summary>
/// 服务器日志初始化服务（请先添加此服务）
/// </summary>
public class ServerLogInitializer : ServerInitializerServiceBase
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        XFEConsole.XFEConsole.UseXFEConsoleLog();
    }
}
