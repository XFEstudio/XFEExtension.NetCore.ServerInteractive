using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

/// <summary>
/// Echo服务：将客户端发送的消息原样返回
/// </summary>
public partial class EchoCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("echo")]
    public async Task EchoEntryPoint()
    {
        var message = Json?["message"]?.GetValue<string>() ?? string.Empty;
        await Close(message);
    }
}
