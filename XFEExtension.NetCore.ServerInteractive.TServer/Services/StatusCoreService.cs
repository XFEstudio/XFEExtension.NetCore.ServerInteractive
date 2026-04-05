using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

/// <summary>
/// 状态服务：返回服务器状态信息
/// </summary>
public partial class StatusCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("status")]
    public async Task StatusEntryPoint()
    {
        var version = typeof(StatusCoreService).Assembly.GetName().Version?.ToString() ?? "unknown";
        await Close(new
        {
            status = "running",
            timestamp = DateTime.UtcNow,
            version
        });
    }
}
