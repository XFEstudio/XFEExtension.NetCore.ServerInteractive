using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

/// <summary>
/// 问候服务：根据名称返回问候语，支持错误处理
/// </summary>
public partial class GreetCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("greet")]
    public async Task GreetEntryPoint()
    {
        var name = Json?["name"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(name)) throw Error("名称不能为空");
        await Close(new { greeting = $"你好, {name}!" });
    }
}
