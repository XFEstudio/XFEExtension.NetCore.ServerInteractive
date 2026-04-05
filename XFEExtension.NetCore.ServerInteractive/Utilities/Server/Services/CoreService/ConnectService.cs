using System.Globalization;
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 连接校验服务
/// </summary>
public partial class ConnectService : ServerCoreStandardServiceBase
{
    /// <summary>
    /// 连接检查入口点
    /// </summary>
    [EntryPoint("connect")]
    public async Task Connect()
    {
        
        Console.Write("检查连接");
        await Close(DateTime.Now.ToString(CultureInfo.CurrentCulture));
    }
}