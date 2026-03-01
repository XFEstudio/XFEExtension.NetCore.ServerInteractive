using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

public class TestCoreService : ServerCoreStandardAsyncServiceBase
{
    public override async Task StandardRequestReceived()
    {
        Console.Write($"收到方法{Execute}");
        await Close("完成");
    }
}
