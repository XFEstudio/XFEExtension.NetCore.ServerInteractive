using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 连接校验服务
/// </summary>
public class ConnectService : ServerCoreStandardAsyncServiceBase
{
    /// <inheritdoc/>
    public override async Task StandardRequestReceived()
    {
        Console.Write("检查连接");
        await Close(DateTime.Now.ToString());
    }
}
