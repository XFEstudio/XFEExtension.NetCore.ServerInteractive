using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

/// <summary>
/// 时间服务：返回服务器当前UTC时间
/// </summary>
public partial class TimeCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("time")]
    public async Task TimeEntryPoint()
    {
        await Close(new { utcNow = DateTime.UtcNow.ToString("o") });
    }
}
