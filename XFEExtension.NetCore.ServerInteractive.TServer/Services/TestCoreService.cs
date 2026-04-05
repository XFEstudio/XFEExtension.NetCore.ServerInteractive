using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

public partial class TestCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("test")]
    public async Task TestEntryPoint()
    {
        Console.Write($"收到测试请求");
        await Close("完成");
    }
}
