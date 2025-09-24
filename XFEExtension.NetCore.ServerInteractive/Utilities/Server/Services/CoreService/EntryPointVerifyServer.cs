using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Profiles;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 服务器入口点校验
/// </summary>
public class EntryPointVerifyServer : ServerCoreVerifyServiceBase
{
    /// <inheritdoc/>
    public override bool VerifyRequest(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r)
    {
        Console.Write($"[DEBUG]【{e.ClientIP}】接收到请求");
        if (ServerBaseProfile.BannedIPAddressList.Contains(e.ClientIP))
        {
            Console.WriteLine("-校验失败");
            r.Error("您的已被封禁", HttpStatusCode.Forbidden);
        }
        if (e.RequestURL?.Segments.Length < 2 || $"{e.RequestURL?.Segments[0]}{e.RequestURL?.Segments[1]}" != $"/{ServerBaseProfile.EntryPoint}" || e.RequestMethod != "POST")
        {
            Console.WriteLine("-校验失败");
            r.Error("请求的API接口不正确", HttpStatusCode.BadGateway);
        }
        Console.WriteLine("-校验通过");
        return true;
    }
}
