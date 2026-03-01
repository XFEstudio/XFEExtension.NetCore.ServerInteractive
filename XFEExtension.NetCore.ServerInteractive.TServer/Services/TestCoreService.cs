using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

public class TestCoreService : ServerCoreStandardServiceBase
{
    public override async Task RequestReceiveAsync()
    {
        Console.Write($"收到方法{Execute}");
        await Close("完成");
    }
}
