using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

/// <summary>
/// 数学运算服务：支持加法和乘法运算
/// </summary>
public partial class MathCoreService : ServerCoreStandardServiceBase
{
    [EntryPoint("math/add")]
    public async Task AddEntryPoint()
    {
        var a = Json?["a"]?.GetValue<double>() ?? 0;
        var b = Json?["b"]?.GetValue<double>() ?? 0;
        await Close(new { result = a + b });
    }

    [EntryPoint("math/multiply")]
    public async Task MultiplyEntryPoint()
    {
        var a = Json?["a"]?.GetValue<double>() ?? 0;
        var b = Json?["b"]?.GetValue<double>() ?? 0;
        await Close(new { result = a * b });
    }
}
